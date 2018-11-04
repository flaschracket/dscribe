using Brainvest.Dscribe.Abstractions;
using Brainvest.Dscribe.Abstractions.Models;
using Brainvest.Dscribe.Abstractions.Models.ManageMetadata;
using Brainvest.Dscribe.MetadataDbAccess;
using Brainvest.Dscribe.MetadataDbAccess.Entities;
using Brainvest.Dscribe.Runtime.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brainvest.Dscribe.Runtime.Controllers
{
	[ApiController]
	[Produces("application/json")]
	[Route("api/[controller]/[action]")]
	public class ManageMetadataController : ControllerBase
	{
		MetadataDbContext _dbContext;
		IImplementationsContainer _implementations;
		IPermissionService _permissionService;

		public ManageMetadataController(
			MetadataDbContext dbContext,
			IImplementationsContainer implementations,
			IPermissionService permissionService)
		{
			_dbContext = dbContext;
			_implementations = implementations;
			_permissionService = permissionService;
		}

		[HttpPost]
		public async Task<ActionResult<IEnumerable<EntityTypeModel>>> GetEntityTypes()
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}

			var validationMessage = await MetadataValidationLogic.GetTypesValidation(_dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}

			var appTypeId = _implementations.InstanceInfo.AppTypeId;
			var entityTypes = await _dbContext.EntityTypes.OrderBy(x => x.Name)
					.Where(x => x.AppTypeId == appTypeId)
					.Select(x => new EntityTypeModel
					{
						BaseEntityTypeId = x.BaseEntityTypeId,
						CodePath = x.CodePath,
						DisplayNamePath = x.DisplayNamePath,
						Id = x.Id,
						Name = x.Name,
						SchemaName = x.SchemaName,
						SingularTitle = x.SingularTitle,
						TableName = x.TableName,
						PluralTitle = x.PluralTitle,
						EntityTypeGeneralUsageCategoryId = x.GeneralUsageCategoryId
					})
					.ToListAsync();
			return entityTypes;
		}

		[HttpPost]
		public async Task<ActionResult> AddEntityType(EntityTypeModel model)
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}

			var validationMessage = await MetadataValidationLogic.AddEntityTypeValidation(model, _dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}

			var type = new EntityType
			{
				BaseEntityTypeId = model.BaseEntityTypeId,
				CodePath = model.CodePath,
				DisplayNamePath = model.DisplayNamePath,
				Name = model.Name,
				SchemaName = model.SchemaName,
				SingularTitle = model.SingularTitle,
				PluralTitle = model.PluralTitle,
				GeneralUsageCategoryId = model.EntityTypeGeneralUsageCategoryId,
				AppTypeId = _implementations.InstanceInfo.AppTypeId,
				TableName = model.TableName
			};
			_dbContext.EntityTypes.Add(type);
			await _dbContext.SaveChangesAsync();
			return Ok();
		}

		[HttpPost]
		public async Task<ActionResult> EditEntityType(EntityTypeModel model)
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}
			var error = await MetadataValidationLogic.EditEntityTypeValidation(model, _dbContext);
			if (error != null)
			{
				return BadRequest(error);
			}

			var entityType = await _dbContext.EntityTypes.FindAsync(model.Id);
			entityType.BaseEntityTypeId = model.BaseEntityTypeId;
			entityType.CodePath = model.CodePath;
			entityType.DisplayNamePath = model.DisplayNamePath;
			entityType.Name = model.Name;
			entityType.SchemaName = model.SchemaName;
			entityType.SingularTitle = model.SingularTitle;
			entityType.PluralTitle = model.PluralTitle;
			entityType.TableName = model.TableName;
			entityType.GeneralUsageCategoryId = model.EntityTypeGeneralUsageCategoryId;
			await _dbContext.SaveChangesAsync();
			return Ok();
		}

		[HttpPost]
		public async Task<ActionResult> DeleteEntityType(EntityTypeModel model)
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}
			var validationMessage = await MetadataValidationLogic.DeleteEntityValidation(model, _dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}
			var entityType = await _dbContext.EntityTypes.FindAsync(model.Id);
			_dbContext.EntityTypes.Remove(entityType);
			await _dbContext.SaveChangesAsync();
			return Ok();
		}

		[HttpPost]
		public async Task<ActionResult<IEnumerable<PropertyModel>>> GetProperties(EntityTypeDetailsRequest request)
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}

			var validationMessage = await MetadataValidationLogic.GetPropertiesValidation(request, _dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}

			var properties = await _dbContext.Properties
					.Where(x => x.OwnerEntityTypeId == request.EntityTypeId).OrderBy(x => x.Name)
					.Select(x => new PropertyModel
					{
						DataTypeId = (int?)x.DataTypeId,
						DataEntityTypeId = x.DataEntityTypeId,
						Id = x.Id,
						IsNullable = x.IsNullable,
						Name = x.Name,
						Title = x.Title,
						PropertyGeneralUsageCategoryId = x.GeneralUsageCategoryId,
						OwnerEntityTypeId = x.OwnerEntityTypeId,
						ForeignKeyPropertyId = x.ForeignKeyPropertyId,
						InversePropertyId = x.InversePropertyId
					}).ToListAsync();
			return properties;
		}

		public async Task<ActionResult<AddNEditPropertyModel>> GetPropertyForEdit(PropertyDetailsRequest request)
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}

			var validationMessage = await MetadataValidationLogic.GetPropertyForEditValidation(request, _dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}

			var model = await _dbContext.Properties.Select(x => new AddNEditPropertyModel
			{
				DataEntityTypeId = x.DataEntityTypeId,
				DataTypeId = (int)x.DataTypeId,
				OwnerEntityTypeId = x.OwnerEntityTypeId,
				ForeignKeyPropertyId = x.ForeignKeyPropertyId,
				NewForeignKeyName = x.ForeignKeyProperty.Name,
				Id = x.Id,
				InversePropertyId = x.InversePropertyId,
				IsNullable = x.IsNullable,
				Name = x.Name,
				PropertyGeneralUsageCategoryId = x.GeneralUsageCategoryId,
				Title = x.Title
			}).SingleOrDefaultAsync(x => x.Id == request.PropertyId);
			if (model.DataTypeId == (int)DataTypeEnum.NavigationEntity)
			{
				var inverse = await _dbContext.Properties.SingleOrDefaultAsync(x => x.InversePropertyId == model.Id);
				if (inverse != null)
				{
					model.InversePropertyId = inverse.Id;
					model.NewInversePropertyTitle = inverse.Title;
					model.NewInversePropertyName = inverse.Name;
				}
				model.InversePropertyAction = model.ForeignKeyAction = RelatedPropertyAction.DontChange;
				model.NewForeignKeyId = model.ForeignKeyPropertyId;
				model.NewInversePropertyId = model.InversePropertyId;
			}
			return model;
		}

		[HttpPost]
		public async Task<ActionResult> AddProperty(AddNEditPropertyModel model)
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}

			var validationMessage = await MetadataValidationLogic.AddPropertyValidation(model, _dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}
			using (var transaction = await _dbContext.Database.BeginTransactionAsync())
			{
				// Why Transaction in here ? ( Arash )
				var property = new Property
				{
					DataTypeId = (DataTypeEnum?)model.DataTypeId,
					DataEntityTypeId = model.DataEntityTypeId,
					IsNullable = model.IsNullable,
					Name = model.Name,
					Title = model.Title,
					GeneralUsageCategoryId = model.PropertyGeneralUsageCategoryId,
					OwnerEntityTypeId = model.OwnerEntityTypeId,
					InversePropertyId = model.InversePropertyId
				};
				if (property.DataTypeId == DataTypeEnum.NavigationEntity)
				{
					await HandleForeignKey(model, property);
					await HandleInvserseProperty(model, property);
				}
				else
				{
					property.ForeignKeyPropertyId = model.NewForeignKeyId;
					property.InversePropertyId = model.NewInversePropertyId;
				}
				_dbContext.Properties.Add(property);
				await _dbContext.SaveChangesAsync();
				transaction.Commit();
			}
			return Ok();
		}

		[HttpPost]
		public async Task<ActionResult> EditProperty(AddNEditPropertyModel model)
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}
			var validationMessage = await MetadataValidationLogic.EditPropertyValidation(model, _dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}

			var property = await _dbContext.Properties.FindAsync(model.Id);
			property.DataTypeId = (DataTypeEnum?)model.DataTypeId;
			property.DataEntityTypeId = model.DataEntityTypeId;
			property.IsNullable = model.IsNullable;
			property.Name = model.Name;
			property.Title = model.Title;
			property.GeneralUsageCategoryId = model.PropertyGeneralUsageCategoryId;
			if (property.DataTypeId == DataTypeEnum.NavigationEntity)
			{
				await HandleForeignKey(model, property);
				await HandleInvserseProperty(model, property);
			}
			else
			{
				property.ForeignKeyPropertyId = model.NewForeignKeyId;
				property.InversePropertyId = model.NewInversePropertyId;
			}
			await _dbContext.SaveChangesAsync();
			return Ok();
		}

		private async Task HandleForeignKey(AddNEditPropertyModel model, Property property)
		{
			switch (model.ForeignKeyAction)
			{
				case RelatedPropertyAction.DontChange:
					return;
				case RelatedPropertyAction.ChooseExistingById:
					property.ForeignKeyPropertyId = model.NewForeignKeyId;
					return;
				case RelatedPropertyAction.RenameExisting:
					var existingForeignKey = await _dbContext.Properties.FindAsync(model.ForeignKeyPropertyId);
					existingForeignKey.Name = model.NewForeignKeyName;
					return;
				case RelatedPropertyAction.CreateNewByName:
					var newForeignKey = new Property
					{
						DataTypeId = DataTypeEnum.ForeignKey,
						DataEntityTypeId = model.DataEntityTypeId,
						OwnerEntityTypeId = property.OwnerEntityTypeId,
						GeneralUsageCategoryId = (await _dbContext.PropertyGeneralUsageCategories.FirstAsync(x => x.Name.Contains("ForeignKey"))).Id,
						IsNullable = model.IsNullable,
						Name = model.NewForeignKeyName,
						Title = model.Title
					};
					property.ForeignKeyProperty = newForeignKey;
					await _dbContext.Properties.AddAsync(newForeignKey);
					return;
				default:
					throw new NotImplementedException();
			}
		}

		private async Task HandleInvserseProperty(AddNEditPropertyModel model, Property property)
		{
			switch (model.InversePropertyAction)
			{
				case RelatedPropertyAction.DontChange:
					return;
				case RelatedPropertyAction.ChooseExistingById:
					property.InversePropertyId = model.NewInversePropertyId;
					return;
				case RelatedPropertyAction.RenameExisting:
					var existingInverseProperty = await _dbContext.Properties.FindAsync(model.InversePropertyId);
					existingInverseProperty.Name = model.NewForeignKeyName;
					existingInverseProperty.Title = model.NewInversePropertyTitle;
					return;
				case RelatedPropertyAction.CreateNewByName:
					var newInverseProperty = new Property
					{
						DataTypeId = DataTypeEnum.NavigationList,
						DataEntityTypeId = model.OwnerEntityTypeId,
						OwnerEntityTypeId = property.DataEntityTypeId.Value,
						GeneralUsageCategoryId = (await _dbContext.PropertyGeneralUsageCategories.FirstAsync(x => x.Name.Contains("NavigationList"))).Id,
						Name = model.NewInversePropertyName,
						Title = model.NewInversePropertyTitle
					};
					newInverseProperty.InverseProperty = property;
					await _dbContext.Properties.AddAsync(newInverseProperty);
					return;
				default:
					throw new NotImplementedException();
			}
		}

		[HttpPost]
		public async Task<ActionResult> DeleteProperty(PropertyModel model)
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}
			var validationMessage = await MetadataValidationLogic.DeletePropertyValidation(model, _dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}
			var property = await _dbContext.Properties.FindAsync(model.Id);
			_dbContext.Properties.Remove(property);
			await _dbContext.SaveChangesAsync();
			return Ok();
		}

		[HttpPost]
		public async Task<ActionResult<MetadataBasicInfoModel>> GetBasicInfo()
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}

			var validationMessage = await MetadataValidationLogic.GetBasicInfoValidation(_dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}
			var result = new MetadataBasicInfoModel
			{
				DefaultPropertyFacetValues = (await _dbContext.PropertyFacetDefaultValues
					.Select(v => new
					{
						UsageCategoryName = v.GeneralUsageCategory.Name,
						FacetName = v.FacetDefinition.Name,
						v.DefaultValue
					})
					.ToListAsync())
					.GroupBy(v => v.UsageCategoryName)
					.ToDictionary(v => v.Key, v => v.ToDictionary(g => g.FacetName, g => g.DefaultValue)),
				DefaultEntityTypeFacetValues = (await _dbContext.EntityTypeFacetDefaultValues
					.Select(v => new
					{
						UsageCategoryName = v.GeneralUsageCategory.Name,
						FacetName = v.FacetDefinition.Name,
						v.DefaultValue
					})
					.ToListAsync())
					.GroupBy(v => v.UsageCategoryName)
					.ToDictionary(v => v.Key, v => v.ToDictionary(g => g.FacetName, g => g.DefaultValue)),
				PropertyFacetDefinitions = await _dbContext.PropertyFacetDefinitions.Select(p => new FacetDefinitionModel
				{
					Id = p.Id,
					Name = p.Name,
					DataType = p.FacetType.Identifier
				}).ToListAsync(),
				EntityTypeFacetDefinitions = await _dbContext.EntityTypeFacetDefinitions.Select(p => new FacetDefinitionModel
				{
					Id = p.Id,
					Name = p.Name,
					DataType = p.FacetType.Identifier
				}).ToListAsync(),
				PropertyGeneralUsageCategories = await _dbContext.PropertyGeneralUsageCategories.Select(u => new GeneralUsageCategoryModel
				{
					Id = u.Id,
					Name = u.Name
				}).ToListAsync(),
				EntityTypeGeneralUsageCategories = await _dbContext.EntityTypeGeneralUsageCategories.Select(u => new GeneralUsageCategoryModel
				{
					Id = u.Id,
					Name = u.Name
				}).ToListAsync(),
				DataTypes = await _dbContext.DataTypes.Select(d => new DataTypeModel
				{
					Id = (int)d.Id,
					Identifier = d.Identifier,
					Name = d.Name
				}).ToListAsync(),
				FacetTypes = await _dbContext.FacetTypes.Select(f => new FacetTypeModel
				{
					Id = (int)f.Id,
					Identifier = f.Identifier,
					Name = f.Name
				}).ToListAsync()
			};
			return result;
		}

		[HttpPost]
		public async Task<ActionResult<LocalFacetsModel>> GetTypeFacets()
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}

			var validationMessage = await MetadataValidationLogic.GetTypeFacetsValidation(_dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}
			var result = new LocalFacetsModel()
			{
				LocalFacets = (await _dbContext.EntityTypeFacetValues.Select(v => new
				{
					v.Id,
					TypeName = v.EntityType.Name,
					FacetName = v.FacetDefinition.Name,
					v.Value
				}).ToListAsync())
				.GroupBy(v => v.TypeName)
				.ToDictionary(v => v.Key, v => v.ToDictionary(g => g.FacetName, g => g.Value))
			};
			return result;
		}

		[HttpPost]
		public async Task<ActionResult<LocalFacetsModel>> GetPropertyFacets(EntityTypeDetailsRequest request)
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}

			var validationMessage = await MetadataValidationLogic.GetPropertyFacetsValidation(request, _dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}
			var result = new LocalFacetsModel()
			{
				LocalFacets = (await _dbContext.PropertyFacetValues.Where(x => x.Property.OwnerEntityTypeId == request.EntityTypeId)
				.Select(v => new
				{
					v.Id,
					TypeName = v.Property.Name,
					FacetName = v.FacetDefinition.Name,
					v.Value
				}).ToListAsync())
				.GroupBy(v => v.TypeName)
				.ToDictionary(v => v.Key, v => v.ToDictionary(g => g.FacetName, g => g.Value))
			};
			return result;
		}

		[HttpPost]
		public async Task<ActionResult> SaveTypeLocalFacetValue(SaveLocalFacetRequest request)
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}

			var validationMessage = await MetadataValidationLogic.SaveTypeLocalFacetValueValidation(request, _dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}
			var existing = await _dbContext.EntityTypeFacetValues
				.Where(x =>
					x.EntityType.Name == request.EntityTypeName
					&& x.FacetDefinition.Name == request.FacetName)
				.SingleOrDefaultAsync();
			if (request.ClearLocalValue)
			{
				if (existing != null)
				{
					_dbContext.EntityTypeFacetValues.Remove(existing);
				}
			}
			else
			{
				if (existing == null)
				{
					existing = new EntityTypeFacetValue
					{
						FacetDefinition = await _dbContext.EntityTypeFacetDefinitions.SingleAsync(x => x.Name == request.FacetName),
						EntityType = await _dbContext.EntityTypes.SingleAsync(x => x.Name == request.EntityTypeName)
					};
					_dbContext.EntityTypeFacetValues.Add(existing);
				}
				existing.Value = request.Value;
			}
			await _dbContext.SaveChangesAsync();
			return Ok();
		}

		[HttpPost]
		public async Task<ActionResult> SavePropertyLocalFacetValue(SaveLocalFacetRequest request)
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}
			var validationMessage = await MetadataValidationLogic.SavePropertyLocalFacetValueValidation(request, _dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}
			var existing = await _dbContext.PropertyFacetValues
				.Where(x =>
				x.Property.OwnerEntityType.Name == request.EntityTypeName
				&& x.Property.Name == request.PropertyName
				&& x.FacetDefinition.Name == request.FacetName)
				.SingleOrDefaultAsync();
			if (request.ClearLocalValue)
			{
				if (existing != null)
				{
					_dbContext.PropertyFacetValues.Remove(existing);
				}
			}
			else
			{
				if (existing == null)
				{
					existing = new PropertyFacetValue
					{
						FacetDefinition = await _dbContext.PropertyFacetDefinitions.SingleAsync(x => x.Name == request.FacetName),
						Property = await _dbContext.Properties.SingleAsync(x => x.OwnerEntityType.Name == request.EntityTypeName && x.Name == request.PropertyName)
					};
					_dbContext.PropertyFacetValues.Add(existing);
				}
				existing.Value = request.Value;
			}
			await _dbContext.SaveChangesAsync();
			return Ok();
		}

		[HttpPost]
		public async Task<ActionResult<IEnumerable<ManageMetadataPropertyInfoModel>>> GetAllPropertyNames()
		{
			if (!_permissionService.IsAllowed(new ActionRequestInfo(HttpContext, _implementations, null, ActionTypeEnum.ManageMetadata)))
			{
				return Unauthorized();
			}
			var validationMessage = await MetadataValidationLogic.GetAllPropertyNamesValidation(_dbContext);
			if (!string.IsNullOrEmpty(validationMessage))
			{
				return StatusCode(400, validationMessage);
			}
			var appTypeId = _implementations.InstanceInfo.AppTypeId;
			var names = await _dbContext.Properties.Where(x => x.OwnerEntityType.AppTypeId == appTypeId)
				.Select(x => new ManageMetadataPropertyInfoModel
				{
					Id = x.Id,
					Name = x.Name,
					DataEntityTypeId = x.DataEntityTypeId,
					DataTypeId = (int)x.DataTypeId,
					OwnerEntityTypeId = x.OwnerEntityTypeId
				}).ToListAsync();
			return names;
		}
	}
}