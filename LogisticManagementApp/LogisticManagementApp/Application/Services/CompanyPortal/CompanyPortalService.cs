using LogisticManagementApp.Applicationn.Interfaces.CompanyPortal;
using LogisticManagementApp.Domain.Billing;
using LogisticManagementApp.Domain.Companies;
using LogisticManagementApp.Domain.Enums.Billing;
using LogisticManagementApp.Domain.Enums.Orders;
using LogisticManagementApp.Domain.Enums.Shipments;
using LogisticManagementApp.Domain.Orders;
using LogisticManagementApp.Domain.Pricing;
using LogisticManagementApp.Domain.Shipments;
using LogisticManagementApp.Infrastructure.Persistence;
using LogisticManagementApp.Models.CompanyPortal;
using LogisticManagementApp.Models.CompanyPortal.Billing;
using LogisticManagementApp.Models.CompanyPortal.Branches;
using LogisticManagementApp.Models.CompanyPortal.Capabilities;
using LogisticManagementApp.Models.CompanyPortal.Contacts;
using LogisticManagementApp.Models.CompanyPortal.Orders;
using LogisticManagementApp.Models.CompanyPortal.Pricing;
using LogisticManagementApp.Models.CompanyPortal.Shipments;
using LogisticManagementApp.Models.CompanyPortal.Shipments.CargoItems;
using LogisticManagementApp.Models.CompanyPortal.Shipments.Common;
using LogisticManagementApp.Models.CompanyPortal.Shipments.Containers;
using LogisticManagementApp.Models.CompanyPortal.Shipments.Legs;
using LogisticManagementApp.Models.CompanyPortal.Shipments.PackageItems;
using LogisticManagementApp.Models.CompanyPortal.Shipments.Packages;
using LogisticManagementApp.Models.CompanyPortal.Shipments.Party;
using LogisticManagementApp.Models.CompanyPortal.Shipments.ProofOfDelivery;
using LogisticManagementApp.Models.CompanyPortal.Shipments.References;
using LogisticManagementApp.Models.CompanyPortal.Shipments.Statuses;
using LogisticManagementApp.Models.CompanyPortal.Shipments.Tags;
using LogisticManagementApp.Models.CompanyPortal.Shipments.TrackingEvents;
using LogisticManagementApp.Models.CompanyPortal.Shipments.Trips;
using LogisticManagementApp.Models.CompanyPortal.Shipments.Ulds;
using LogisticManagementApp.Models.CompanyPortal.Shipments.Voyages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LogisticManagementApp.Applicationn.Services.CompanyPortal
{
    public partial class CompanyPortalService : ICompanyPortalService
    {
        private readonly LogisticAppDbContext _dbContext;
        private readonly ISequentialNumberGenerator _numberGenerator;

        public CompanyPortalService(LogisticAppDbContext dbContext, ISequentialNumberGenerator numberGenerator)
        {
            _dbContext = dbContext;
            _numberGenerator = numberGenerator;
        }

        // Company Profile

        #region Company Profile

        public async Task<CompanyProfileViewModel?> GetMyCompanyAsync(Guid companyId)
        {
            return await _dbContext.Companies
                .Where(x => x.Id == companyId)
                .Select(x => new CompanyProfileViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    TaxNumber = x.TaxNumber,
                    CompanyType = x.CompanyType.ToString(),
                    Website = x.Website,
                    Notes = x.Notes,
                    IsActive = x.IsActive
                })
                .FirstOrDefaultAsync();
        }
        public async Task<EditCompanyProfileViewModel?> GetEditCompanyAsync(Guid companyId)
        {
            return await _dbContext.Companies
                .Where(x => x.Id == companyId)
                .Select(x => new EditCompanyProfileViewModel
                {
                    Name = x.Name,
                    TaxNumber = x.TaxNumber,
                    CompanyType = x.CompanyType,
                    Website = x.Website,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> UpdateMyCompanyAsync(Guid companyId, EditCompanyProfileViewModel model)
        {
            var company = await _dbContext.Companies
                .FirstOrDefaultAsync(x => x.Id == companyId);

            if (company == null)
            {
                return false;
            }

            company.Name = model.Name;
            company.TaxNumber = model.TaxNumber;
            company.CompanyType = model.CompanyType;
            company.Website = model.Website;
            company.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        
        #endregion

        // Shared Options

        public async Task<IEnumerable<SelectListItem>> GetAddressOptionsAsync()
        {
            return await _dbContext.Addresses
                .OrderBy(x => x.PostalCode)
                .ThenBy(x => x.City)
                .ThenBy(x => x.Street)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.Street}, {x.City}, {x.PostalCode}"
                })
                .ToListAsync();
        }

        // Branches

        #region Branches

        public async Task<IEnumerable<CompanyBranchListItemViewModel>> GetMyBranchesAsync(Guid companyId)
        {
            return await _dbContext.CompanyBranches
                .Where(x => x.CompanyId == companyId)
                .OrderByDescending(x => x.IsHeadOffice)
                .ThenBy(x => x.Name)
                .Select(x => new CompanyBranchListItemViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    AddressId = x.AddressId,
                    AddressText = x.Address != null
                        ? $"{x.Address.Street}, {x.Address.City}, {x.Address.PostalCode}"
                        : null,
                    BranchCode = x.BranchCode,
                    Phone = x.Phone,
                    Email = x.Email,
                    IsHeadOffice = x.IsHeadOffice,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();
        }
        public async Task CreateBranchAsync(Guid companyId, CompanyBranchCreateViewModel model)
        {
            if (model.IsHeadOffice)
            {
                var existingHeadOffices = await _dbContext.CompanyBranches
                    .Where(x => x.CompanyId == companyId && x.IsHeadOffice)
                    .ToListAsync();

                foreach (var existing in existingHeadOffices)
                {
                    existing.IsHeadOffice = false;
                }
            }

            var hasAnyBranches = await _dbContext.CompanyBranches
                .AnyAsync(x => x.CompanyId == companyId);

            var branch = new CompanyBranch
            {
                CompanyId = companyId,
                Name = model.Name,
                AddressId = model.AddressId,
                BranchCode = model.BranchCode,
                Phone = model.Phone,
                Email = model.Email,
                IsHeadOffice = !hasAnyBranches || model.IsHeadOffice,
                IsActive = model.IsActive,
                Notes = model.Notes
            };

            await _dbContext.CompanyBranches.AddAsync(branch);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<CompanyBranchEditViewModel?> GetBranchForEditAsync(Guid companyId, Guid branchId)
        {
            return await _dbContext.CompanyBranches
                .Where(x => x.Id == branchId && x.CompanyId == companyId)
                .Select(x => new CompanyBranchEditViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    AddressId = x.AddressId,
                    BranchCode = x.BranchCode,
                    Phone = x.Phone,
                    Email = x.Email,
                    IsHeadOffice = x.IsHeadOffice,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> UpdateBranchAsync(Guid companyId, CompanyBranchEditViewModel model)
        {
            var branch = await _dbContext.CompanyBranches
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.CompanyId == companyId);

            if (branch == null)
            {
                return false;
            }

            if (model.IsHeadOffice)
            {
                var otherHeadOffices = await _dbContext.CompanyBranches
                    .Where(x => x.CompanyId == companyId && x.Id != model.Id && x.IsHeadOffice)
                    .ToListAsync();

                foreach (var other in otherHeadOffices)
                {
                    other.IsHeadOffice = false;
                }
            }

            branch.Name = model.Name;
            branch.AddressId = model.AddressId;
            branch.BranchCode = model.BranchCode;
            branch.Phone = model.Phone;
            branch.Email = model.Email;
            branch.IsHeadOffice = model.IsHeadOffice;
            branch.IsActive = model.IsActive;
            branch.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteBranchAsync(Guid companyId, Guid branchId)
        {
            var branch = await _dbContext.CompanyBranches
                .FirstOrDefaultAsync(x => x.Id == branchId && x.CompanyId == companyId);

            if (branch == null)
            {
                return false;
            }

            var wasHeadOffice = branch.IsHeadOffice;

            _dbContext.CompanyBranches.Remove(branch);
            await _dbContext.SaveChangesAsync();

            if (wasHeadOffice)
            {
                var nextBranch = await _dbContext.CompanyBranches
                    .Where(x => x.CompanyId == companyId)
                    .OrderBy(x => x.Name)
                    .FirstOrDefaultAsync();

                if (nextBranch != null)
                {
                    nextBranch.IsHeadOffice = true;
                    await _dbContext.SaveChangesAsync();
                }
            }

            return true;
        }

        #endregion

        // Contacts
        
        #region Contacts

        public async Task<IEnumerable<CompanyContactListItemViewModel>> GetMyContactsAsync(Guid companyId)
        {
            return await _dbContext.CompanyContacts
                .Where(x => x.CompanyId == companyId)
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.FullName)
                .Select(x => new CompanyContactListItemViewModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    Phone = x.Phone,
                    RoleTitle = x.RoleTitle,
                    IsPrimary = x.IsPrimary
                })
                .ToListAsync();
        }
        public async Task CreateContactAsync(Guid companyId, CompanyContactCreateViewModel model)
        {
            if (model.IsPrimary)
            {
                var existingPrimaryContacts = await _dbContext.CompanyContacts
                    .Where(x => x.CompanyId == companyId && x.IsPrimary)
                    .ToListAsync();

                foreach (var existing in existingPrimaryContacts)
                {
                    existing.IsPrimary = false;
                }
            }

            var hasAnyContacts = await _dbContext.CompanyContacts
                .AnyAsync(x => x.CompanyId == companyId);

            var contact = new CompanyContact
            {
                CompanyId = companyId,
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                RoleTitle = model.RoleTitle,
                IsPrimary = !hasAnyContacts || model.IsPrimary
            };

            await _dbContext.CompanyContacts.AddAsync(contact);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<CompanyContactEditViewModel?> GetContactForEditAsync(Guid companyId, Guid contactId)
        {
            return await _dbContext.CompanyContacts
                .Where(x => x.Id == contactId && x.CompanyId == companyId)
                .Select(x => new CompanyContactEditViewModel
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    Phone = x.Phone,
                    RoleTitle = x.RoleTitle,
                    IsPrimary = x.IsPrimary
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> UpdateContactAsync(Guid companyId, CompanyContactEditViewModel model)
        {
            var contact = await _dbContext.CompanyContacts
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.CompanyId == companyId);

            if (contact == null)
            {
                return false;
            }

            if (model.IsPrimary)
            {
                var otherPrimaryContacts = await _dbContext.CompanyContacts
                    .Where(x => x.CompanyId == companyId && x.Id != model.Id && x.IsPrimary)
                    .ToListAsync();

                foreach (var other in otherPrimaryContacts)
                {
                    other.IsPrimary = false;
                }
            }

            contact.FullName = model.FullName;
            contact.Email = model.Email;
            contact.Phone = model.Phone;
            contact.RoleTitle = model.RoleTitle;
            contact.IsPrimary = model.IsPrimary;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteContactAsync(Guid companyId, Guid contactId)
        {
            var contact = await _dbContext.CompanyContacts
                .FirstOrDefaultAsync(x => x.Id == contactId && x.CompanyId == companyId);

            if (contact == null)
            {
                return false;
            }

            var wasPrimary = contact.IsPrimary;

            _dbContext.CompanyContacts.Remove(contact);
            await _dbContext.SaveChangesAsync();

            if (wasPrimary)
            {
                var nextContact = await _dbContext.CompanyContacts
                    .Where(x => x.CompanyId == companyId)
                    .OrderBy(x => x.FullName)
                    .FirstOrDefaultAsync();

                if (nextContact != null)
                {
                    nextContact.IsPrimary = true;
                    await _dbContext.SaveChangesAsync();
                }
            }

            return true;
        }

        #endregion

        // Capabilities

        #region Capabilities

        public async Task<IEnumerable<CompanyCapabilityListItemViewModel>> GetMyCapabilitiesAsync(Guid companyId)
        {
            return await _dbContext.CompanyCapabilities
                .Where(x => x.CompanyId == companyId)
                .OrderBy(x => x.CapabilityType)
                .Select(x => new CompanyCapabilityListItemViewModel
                {
                    Id = x.Id,
                    CapabilityType = x.CapabilityType.ToString(),
                    IsEnabled = x.IsEnabled,
                    ValidFromUtc = x.ValidFromUtc,
                    ValidToUtc = x.ValidToUtc,
                    Notes = x.Notes
                })
                .ToListAsync();
        }
        public async Task<bool> CreateCapabilityAsync(Guid companyId, CompanyCapabilityCreateViewModel model)
        {
            if (model.ValidFromUtc.HasValue && model.ValidToUtc.HasValue &&
                model.ValidFromUtc > model.ValidToUtc)
            {
                return false;
            }

            var alreadyExists = await _dbContext.CompanyCapabilities
                .AnyAsync(x => x.CompanyId == companyId && x.CapabilityType == model.CapabilityType);

            if (alreadyExists)
            {
                return false;
            }

            var capability = new CompanyCapability
            {
                CompanyId = companyId,
                CapabilityType = model.CapabilityType,
                IsEnabled = model.IsEnabled,
                ValidFromUtc = model.ValidFromUtc,
                ValidToUtc = model.ValidToUtc,
                Notes = model.Notes
            };

            await _dbContext.CompanyCapabilities.AddAsync(capability);
            await _dbContext.SaveChangesAsync();

            return true;
        }
        public async Task<CompanyCapabilityEditViewModel?> GetCapabilityForEditAsync(Guid companyId, Guid capabilityId)
        {
            return await _dbContext.CompanyCapabilities
                .Where(x => x.Id == capabilityId && x.CompanyId == companyId)
                .Select(x => new CompanyCapabilityEditViewModel
                {
                    Id = x.Id,
                    CapabilityType = x.CapabilityType,
                    IsEnabled = x.IsEnabled,
                    ValidFromUtc = x.ValidFromUtc,
                    ValidToUtc = x.ValidToUtc,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }
        public async Task<bool> UpdateCapabilityAsync(Guid companyId, CompanyCapabilityEditViewModel model)
        {
            if (model.ValidFromUtc.HasValue && model.ValidToUtc.HasValue &&
                model.ValidFromUtc > model.ValidToUtc)
            {
                return false;
            }

            var capability = await _dbContext.CompanyCapabilities
                .FirstOrDefaultAsync(x => x.Id == model.Id && x.CompanyId == companyId);

            if (capability == null)
            {
                return false;
            }

            var duplicateExists = await _dbContext.CompanyCapabilities
                .AnyAsync(x => x.CompanyId == companyId &&
                               x.Id != model.Id &&
                               x.CapabilityType == model.CapabilityType);

            if (duplicateExists)
            {
                return false;
            }

            capability.CapabilityType = model.CapabilityType;
            capability.IsEnabled = model.IsEnabled;
            capability.ValidFromUtc = model.ValidFromUtc;
            capability.ValidToUtc = model.ValidToUtc;
            capability.Notes = model.Notes;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteCapabilityAsync(Guid companyId, Guid capabilityId)
        {
            var capability = await _dbContext.CompanyCapabilities
                .FirstOrDefaultAsync(x => x.Id == capabilityId && x.CompanyId == companyId);

            if (capability == null)
            {
                return false;
            }

            _dbContext.CompanyCapabilities.Remove(capability);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        // Orders
        
        #region Orders

        public async Task<CompanyOrdersViewModel> GetMyOrdersAsync(Guid companyId)
        {
            var orders = await _dbContext.Orders
                .AsNoTracking()
                .Where(o => o.CustomerCompanyId == companyId && o.IsActive && !o.IsDeleted)
                .Include(o => o.PickupAddress)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.Lines)
                .OrderByDescending(o => o.CreatedAtUtc)
                .ToListAsync();

            var orderIds = orders.Select(o => o.Id).ToList();

            var shipments = await _dbContext.Shipments
                .AsNoTracking()
                .Where(s => s.OrderId.HasValue && orderIds.Contains(s.OrderId.Value))
                .Select(s => new
                {
                    OrderId = s.OrderId!.Value,
                    ShipmentId = s.Id,
                    ShipmentNo = s.ShipmentNo
                })
                .ToListAsync();

            var shipmentsByOrderId = shipments
                .GroupBy(x => x.OrderId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.ShipmentNo).First());

            return new CompanyOrdersViewModel
            {
                Orders = orders.Select(o =>
                {
                    shipmentsByOrderId.TryGetValue(o.Id, out var shipment);

                    return new CompanyOrderListItemViewModel
                    {
                        Id = o.Id,
                        OrderNo = o.OrderNo,
                        Status = o.Status.ToString(),
                        Priority = o.Priority.ToString(),
                        PickupAddress = o.PickupAddress != null
                            ? $"{o.PickupAddress.Street}, {o.PickupAddress.City}, {o.PickupAddress.Country}"
                            : null,
                        DeliveryAddress = o.DeliveryAddress != null
                            ? $"{o.DeliveryAddress.Street}, {o.DeliveryAddress.City}, {o.DeliveryAddress.Country}"
                            : null,
                        RequestedPickupDateUtc = o.RequestedPickupDateUtc,
                        CustomerReference = o.CustomerReference,
                        LinesCount = o.Lines.Count,
                        ShipmentId = shipment?.ShipmentId,
                        ShipmentNo = shipment?.ShipmentNo
                    };
                }).ToList()
            };
        }
        public async Task<CompanyOrderDetailsViewModel?> GetOrderDetailsAsync(Guid companyId, Guid orderId)
        {
            var order = await _dbContext.Orders
                .AsNoTracking()
                .Where(o => o.Id == orderId && o.CustomerCompanyId == companyId && o.IsActive && !o.IsDeleted)
                .Include(o => o.PickupAddress)
                .Include(o => o.DeliveryAddress)
                .Include(o => o.Lines)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return null;
            }

            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(s => s.OrderId == order.Id)
                .OrderByDescending(s => s.CreatedAtUtc)
                .Select(s => new { s.Id, s.ShipmentNo })
                .FirstOrDefaultAsync();

            var history = await _dbContext.OrderStatusHistories
                .AsNoTracking()
                .Where(x => x.OrderId == order.Id)
                .OrderBy(x => x.ChangedAtUtc)
                .Select(x => new CompanyOrderStatusHistoryItemViewModel
                {
                    Id = x.Id,
                    OldStatus = x.OldStatus.ToString(),
                    NewStatus = x.NewStatus.ToString(),
                    ChangedAtUtc = x.ChangedAtUtc,
                    Reason = x.Reason
                })
                .ToListAsync();

            return new CompanyOrderDetailsViewModel
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                Status = order.Status.ToString(),
                Priority = order.Priority.ToString(),
                PickupAddress = order.PickupAddress != null
                    ? $"{order.PickupAddress.Street}, {order.PickupAddress.City}, {order.PickupAddress.Country}"
                    : null,
                DeliveryAddress = order.DeliveryAddress != null
                    ? $"{order.DeliveryAddress.Street}, {order.DeliveryAddress.City}, {order.DeliveryAddress.Country}"
                    : null,
                RequestedPickupDateUtc = order.RequestedPickupDateUtc,
                CustomerReference = order.CustomerReference,
                Notes = order.Notes,
                IsActive = order.IsActive,
                ShipmentId = shipment?.Id,
                ShipmentNo = shipment?.ShipmentNo,
                CanEdit = CanEditOrder(order.Status),
                CanDelete = order.Status == OrderStatus.Draft,
                CanSubmit = order.Status == OrderStatus.Draft,
                CanConfirm = order.Status == OrderStatus.Submitted,
                CanStartProgress = order.Status == OrderStatus.Confirmed,
                CanComplete = order.Status == OrderStatus.InProgress,
                CanCancel = order.Status != OrderStatus.Completed && order.Status != OrderStatus.Cancelled,
                CanManageLines = CanManageOrderLines(order.Status),
                Lines = order.Lines
                    .OrderBy(x => x.LineNo)
                    .Select(x => new CompanyOrderLineItemViewModel
                    {
                        Id = x.Id,
                        LineNo = x.LineNo,
                        Description = x.Description,
                        Quantity = x.Quantity,
                        QuantityUnit = x.QuantityUnit,
                        GrossWeightKg = x.GrossWeightKg,
                        VolumeCbm = x.VolumeCbm,
                        IsDangerousGoods = x.IsDangerousGoods,
                        HsCode = x.HsCode,
                        OriginCountry = x.OriginCountry
                    }).ToList(),
                StatusHistory = history
            };
        }
        public async Task<CompanyOrderCreateViewModel> GetCreateOrderModelAsync()
        {
            return new CompanyOrderCreateViewModel
            {
                AddressOptions = await GetAddressOptionsAsync()
            };
        }
        public async Task<Guid> CreateOrderAsync(Guid companyId, CompanyOrderCreateViewModel model)
        {
            var orderNo = await _numberGenerator.GenerateOrderNoAsync();

            var order = new Order
            {
                CustomerCompanyId = companyId,
                OrderNo = orderNo,
                PickupAddressId = model.PickupAddressId,
                DeliveryAddressId = model.DeliveryAddressId,
                Priority = model.Priority,
                RequestedPickupDateUtc = model.RequestedPickupDateUtc,
                CustomerReference = model.CustomerReference,
                Notes = model.Notes,
                Status = OrderStatus.Draft,
                IsActive = true
            };

            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            await _dbContext.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                OrderId = order.Id,
                OldStatus = OrderStatus.Draft,
                NewStatus = OrderStatus.Draft,
                ChangedAtUtc = DateTime.UtcNow,
                Reason = "Order created by company."
            });

            await _dbContext.SaveChangesAsync();
            return order.Id;
        }
        public async Task<CompanyOrderEditViewModel?> GetOrderForEditAsync(Guid companyId, Guid orderId)
        {
            var order = await _dbContext.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerCompanyId == companyId && o.IsActive && !o.IsDeleted);

            if (order == null || !CanEditOrder(order.Status))
            {
                return null;
            }

            return new CompanyOrderEditViewModel
            {
                Id = order.Id,
                OrderNo = order.OrderNo,
                Status = order.Status.ToString(),
                PickupAddressId = order.PickupAddressId,
                DeliveryAddressId = order.DeliveryAddressId,
                Priority = order.Priority,
                RequestedPickupDateUtc = order.RequestedPickupDateUtc,
                CustomerReference = order.CustomerReference,
                Notes = order.Notes,
                AddressOptions = await GetAddressOptionsAsync()
            };
        }
        public async Task<bool> UpdateOrderAsync(Guid companyId, CompanyOrderEditViewModel model)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == model.Id && o.CustomerCompanyId == companyId && o.IsActive && !o.IsDeleted);

            if (order == null || !CanEditOrder(order.Status))
            {
                return false;
            }

            order.PickupAddressId = model.PickupAddressId;
            order.DeliveryAddressId = model.DeliveryAddressId;
            order.Priority = model.Priority;
            order.RequestedPickupDateUtc = model.RequestedPickupDateUtc;
            order.CustomerReference = model.CustomerReference;
            order.Notes = model.Notes;
            order.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteOrderAsync(Guid companyId, Guid orderId)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerCompanyId == companyId && o.IsActive && !o.IsDeleted);

            if (order == null || order.Status != OrderStatus.Draft)
            {
                return false;
            }

            order.IsActive = false;
            order.IsDeleted = true;
            order.DeletedAtUtc = DateTime.UtcNow;
            order.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        // Order Lines

        #region Order Lines

        public async Task<CompanyOrderLineCreateViewModel?> GetCreateOrderLineModelAsync(Guid companyId, Guid orderId)
        {
            var order = await _dbContext.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerCompanyId == companyId && o.IsActive && !o.IsDeleted);

            if (order == null || !CanManageOrderLines(order.Status))
            {
                return null;
            }

            var nextLineNo = await _dbContext.OrderLines
                .Where(x => x.OrderId == orderId)
                .Select(x => (int?)x.LineNo)
                .MaxAsync() ?? 0;

            return new CompanyOrderLineCreateViewModel
            {
                OrderId = orderId,
                LineNo = nextLineNo + 1
            };
        }
        public async Task<bool> CreateOrderLineAsync(Guid companyId, CompanyOrderLineCreateViewModel model)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == model.OrderId && o.CustomerCompanyId == companyId && o.IsActive && !o.IsDeleted);

            if (order == null || !CanManageOrderLines(order.Status))
            {
                return false;
            }

            var duplicateLineNo = await _dbContext.OrderLines
                .AnyAsync(x => x.OrderId == model.OrderId && x.LineNo == model.LineNo);

            if (duplicateLineNo)
            {
                return false;
            }

            var line = new OrderLine
            {
                OrderId = model.OrderId,
                LineNo = model.LineNo,
                Description = model.Description,
                Quantity = model.Quantity,
                QuantityUnit = model.QuantityUnit,
                GrossWeightKg = model.GrossWeightKg,
                VolumeCbm = model.VolumeCbm,
                IsDangerousGoods = model.IsDangerousGoods,
                HsCode = model.HsCode,
                OriginCountry = model.OriginCountry
            };

            await _dbContext.OrderLines.AddAsync(line);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<CompanyOrderLineEditViewModel?> GetOrderLineForEditAsync(Guid companyId, Guid lineId)
        {
            var line = await _dbContext.OrderLines
                .AsNoTracking()
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x => x.Id == lineId &&
                                          x.Order.CustomerCompanyId == companyId &&
                                          x.Order.IsActive &&
                                          !x.Order.IsDeleted);

            if (line == null || !CanManageOrderLines(line.Order.Status))
            {
                return null;
            }

            return new CompanyOrderLineEditViewModel
            {
                Id = line.Id,
                OrderId = line.OrderId,
                LineNo = line.LineNo,
                Description = line.Description,
                Quantity = line.Quantity,
                QuantityUnit = line.QuantityUnit,
                GrossWeightKg = line.GrossWeightKg,
                VolumeCbm = line.VolumeCbm,
                IsDangerousGoods = line.IsDangerousGoods,
                HsCode = line.HsCode,
                OriginCountry = line.OriginCountry
            };
        }
        public async Task<bool> UpdateOrderLineAsync(Guid companyId, CompanyOrderLineEditViewModel model)
        {
            var line = await _dbContext.OrderLines
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x => x.Id == model.Id &&
                                          x.Order.CustomerCompanyId == companyId &&
                                          x.Order.IsActive &&
                                          !x.Order.IsDeleted);

            if (line == null || !CanManageOrderLines(line.Order.Status))
            {
                return false;
            }

            var duplicateLineNo = await _dbContext.OrderLines
                .AnyAsync(x => x.OrderId == line.OrderId && x.Id != model.Id && x.LineNo == model.LineNo);

            if (duplicateLineNo)
            {
                return false;
            }

            line.LineNo = model.LineNo;
            line.Description = model.Description;
            line.Quantity = model.Quantity;
            line.QuantityUnit = model.QuantityUnit;
            line.GrossWeightKg = model.GrossWeightKg;
            line.VolumeCbm = model.VolumeCbm;
            line.IsDangerousGoods = model.IsDangerousGoods;
            line.HsCode = model.HsCode;
            line.OriginCountry = model.OriginCountry;
            line.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteOrderLineAsync(Guid companyId, Guid lineId)
        {
            var line = await _dbContext.OrderLines
                .Include(x => x.Order)
                .FirstOrDefaultAsync(x => x.Id == lineId &&
                                          x.Order.CustomerCompanyId == companyId &&
                                          x.Order.IsActive &&
                                          !x.Order.IsDeleted);

            if (line == null || !CanManageOrderLines(line.Order.Status))
            {
                return false;
            }

            _dbContext.OrderLines.Remove(line);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        // Order Status

        #region Order Status

        public async Task<bool> SubmitOrderAsync(Guid companyId, Guid orderId)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Lines)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerCompanyId == companyId && o.IsActive && !o.IsDeleted);

            if (order == null || order.Status != OrderStatus.Draft || !order.Lines.Any())
            {
                return false;
            }

            var oldStatus = order.Status;
            order.Status = OrderStatus.Submitted;
            order.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                OrderId = order.Id,
                OldStatus = oldStatus,
                NewStatus = OrderStatus.Submitted,
                ChangedAtUtc = DateTime.UtcNow,
                Reason = "Submitted by company."
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> ConfirmOrderAsync(Guid companyId, Guid orderId)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId &&
                                          o.CustomerCompanyId == companyId &&
                                          o.IsActive &&
                                          !o.IsDeleted);

            if (order == null || order.Status != OrderStatus.Submitted)
            {
                return false;
            }

            var oldStatus = order.Status;
            order.Status = OrderStatus.Confirmed;
            order.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                OrderId = order.Id,
                OldStatus = oldStatus,
                NewStatus = OrderStatus.Confirmed,
                ChangedAtUtc = DateTime.UtcNow,
                Reason = "Confirmed by company."
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> MarkOrderInProgressAsync(Guid companyId, Guid orderId)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId &&
                                          o.CustomerCompanyId == companyId &&
                                          o.IsActive &&
                                          !o.IsDeleted);

            if (order == null || order.Status != OrderStatus.Confirmed)
            {
                return false;
            }

            var oldStatus = order.Status;
            order.Status = OrderStatus.InProgress;
            order.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                OrderId = order.Id,
                OldStatus = oldStatus,
                NewStatus = OrderStatus.InProgress,
                ChangedAtUtc = DateTime.UtcNow,
                Reason = "Marked as in progress by company."
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CompleteOrderAsync(Guid companyId, Guid orderId)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId &&
                                          o.CustomerCompanyId == companyId &&
                                          o.IsActive &&
                                          !o.IsDeleted);

            if (order == null || order.Status != OrderStatus.InProgress)
            {
                return false;
            }

            var oldStatus = order.Status;
            order.Status = OrderStatus.Completed;
            order.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                OrderId = order.Id,
                OldStatus = oldStatus,
                NewStatus = OrderStatus.Completed,
                ChangedAtUtc = DateTime.UtcNow,
                Reason = "Completed by company."
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CancelOrderAsync(Guid companyId, Guid orderId, string? reason = null)
        {
            var order = await _dbContext.Orders
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerCompanyId == companyId && o.IsActive && !o.IsDeleted);

            if (order == null || order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
            {
                return false;
            }

            var oldStatus = order.Status;
            order.Status = OrderStatus.Cancelled;
            order.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.OrderStatusHistories.AddAsync(new OrderStatusHistory
            {
                OrderId = order.Id,
                OldStatus = oldStatus,
                NewStatus = OrderStatus.Cancelled,
                ChangedAtUtc = DateTime.UtcNow,
                Reason = string.IsNullOrWhiteSpace(reason) ? "Cancelled by company." : reason
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        // Shipments

        #region Shipments

        public async Task<CompanyShipmentCreateViewModel> GetCreateShipmentModelAsync(Guid companyId)
        {
            var model = new CompanyShipmentCreateViewModel();
            await PopulateShipmentFormOptionsAsync(companyId, model);
            return model;
        }
        public async Task<Guid> CreateShipmentAsync(Guid companyId, CompanyShipmentCreateViewModel model)
        {
            var shipment = new Shipment
            {
                ShipmentNo = await _numberGenerator.GenerateShipmentNoAsync(),
                CustomerCompanyId = companyId,
                OrderId = model.OrderId,
                SenderAddressId = model.SenderAddressId,
                ReceiverAddressId = model.ReceiverAddressId,
                Status = model.Status,
                PrimaryMode = model.PrimaryMode,
                Incoterm = model.Incoterm,
                DeclaredValue = model.DeclaredValue,
                Currency = string.IsNullOrWhiteSpace(model.Currency)
                    ? null
                    : model.Currency.Trim().ToUpper(),
                CustomerReference = string.IsNullOrWhiteSpace(model.CustomerReference)
                    ? null
                    : model.CustomerReference.Trim(),
                Notes = string.IsNullOrWhiteSpace(model.Notes)
                    ? null
                    : model.Notes.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _dbContext.Shipments.AddAsync(shipment);

            await _dbContext.ShipmentStatusHistories.AddAsync(new ShipmentStatusHistory
            {
                ShipmentId = shipment.Id,
                OldStatus = shipment.Status,
                NewStatus = shipment.Status,
                ChangedAtUtc = DateTime.UtcNow,
                Reason = "Initial shipment creation from company portal."
            });

            await _dbContext.SaveChangesAsync();
            return shipment.Id;
        }
        public async Task<CompanyShipmentEditViewModel?> GetShipmentForEditAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            x.Id == shipmentId &&
                            x.CustomerCompanyId == companyId)
                .Select(x => new CompanyShipmentEditViewModel
                {
                    Id = x.Id,
                    ShipmentNo = x.ShipmentNo,
                    OrderId = x.OrderId,
                    SenderAddressId = x.SenderAddressId,
                    ReceiverAddressId = x.ReceiverAddressId,
                    PrimaryMode = x.PrimaryMode,
                    Incoterm = x.Incoterm,
                    DeclaredValue = x.DeclaredValue,
                    Currency = x.Currency,
                    CustomerReference = x.CustomerReference,
                    Notes = x.Notes,
                    Status = x.Status.ToString()
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            await PopulateShipmentFormOptionsAsync(companyId, shipment);
            return shipment;
        }
        public async Task<bool> UpdateShipmentAsync(Guid companyId, CompanyShipmentEditViewModel model)
        {
            var shipment = await _dbContext.Shipments
                .FirstOrDefaultAsync(x => !x.IsDeleted &&
                                          x.Id == model.Id &&
                                          x.CustomerCompanyId == companyId);

            if (shipment == null)
            {
                return false;
            }

            shipment.OrderId = model.OrderId;
            shipment.SenderAddressId = model.SenderAddressId;
            shipment.ReceiverAddressId = model.ReceiverAddressId;
            shipment.PrimaryMode = model.PrimaryMode;
            shipment.Incoterm = model.Incoterm;
            shipment.DeclaredValue = model.DeclaredValue;
            shipment.Currency = string.IsNullOrWhiteSpace(model.Currency)
                ? null
                : model.Currency.Trim().ToUpper();
            shipment.CustomerReference = string.IsNullOrWhiteSpace(model.CustomerReference)
                ? null
                : model.CustomerReference.Trim();
            shipment.Notes = string.IsNullOrWhiteSpace(model.Notes)
                ? null
                : model.Notes.Trim();
            shipment.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteShipmentAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await _dbContext.Shipments
                .FirstOrDefaultAsync(x => !x.IsDeleted &&
                                          x.Id == shipmentId &&
                                          x.CustomerCompanyId == companyId);

            if (shipment == null)
            {
                return false;
            }

            shipment.IsDeleted = true;
            shipment.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<CompanyShipmentsViewModel> GetMyShipmentsAsync(Guid companyId)
        {
            var shipments = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Include(s => s.Order)
                .Include(s => s.SenderAddress)
                .Include(s => s.ReceiverAddress)
                .Include(s => s.Legs)
                .OrderByDescending(s => s.CreatedAtUtc)
                .Select(s => new CompanyShipmentListItemViewModel
                {
                    Id = s.Id,
                    ShipmentNo = s.ShipmentNo,
                    Status = s.Status.ToString(),
                    PrimaryMode = s.PrimaryMode.ToString(),
                    OrderNo = s.Order != null ? s.Order.OrderNo : null,
                    PickupAddress = s.SenderAddress != null
                        ? $"{s.SenderAddress.Street}, {s.SenderAddress.City}, {s.SenderAddress.Country}"
                        : null,
                    DeliveryAddress = s.ReceiverAddress != null
                        ? $"{s.ReceiverAddress.Street}, {s.ReceiverAddress.City}, {s.ReceiverAddress.Country}"
                        : null,
                    CustomerReference = s.CustomerReference,
                    CreatedAtUtc = s.CreatedAtUtc,
                    LegsCount = s.Legs.Count,
                    TrackingEventsCount = _dbContext.TrackingEvents.Count(x => x.ShipmentId == s.Id),
                    PackagesCount = _dbContext.Packages.Count(x => x.ShipmentId == s.Id),
                    CargoItemsCount = _dbContext.CargoItems.Count(x => x.ShipmentId == s.Id)
                })
                .ToListAsync();

            return new CompanyShipmentsViewModel { Shipments = shipments };
        }
        public async Task<CompanyShipmentDetailsViewModel?> GetShipmentDetailsAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Include(s => s.Order)
                .Include(s => s.SenderAddress)
                .Include(s => s.ReceiverAddress)
                .Include(s => s.Parties).ThenInclude(p => p.Company)
                .Include(s => s.Legs).ThenInclude(l => l.OriginLocation)
                .Include(s => s.Legs).ThenInclude(l => l.DestinationLocation)
                .FirstOrDefaultAsync(s => s.Id == shipmentId);

            if (shipment == null)
            {
                return null;
            }

            var statusHistory = await _dbContext.ShipmentStatusHistories
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .OrderByDescending(x => x.ChangedAtUtc)
                .Select(x => new CompanyShipmentStatusHistoryItemViewModel
                {
                    OldStatus = x.OldStatus.ToString(),
                    NewStatus = x.NewStatus.ToString(),
                    ChangedAtUtc = x.ChangedAtUtc,
                    Reason = x.Reason
                })
                .ToListAsync();

            var trackingEvents = await _dbContext.TrackingEvents
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .Include(x => x.Location)
                .OrderByDescending(x => x.EventTimeUtc)
                .Select(x => new CompanyTrackingEventItemViewModel
                {
                    EventType = x.EventType.ToString(),
                    EventTimeUtc = x.EventTimeUtc,
                    Location = x.Location != null ? x.Location.Name : null,
                    Details = x.Details,
                    Source = x.Source
                })
                .ToListAsync();

            var pods = await _dbContext.ProofOfDeliveries
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .OrderByDescending(x => x.DeliveredAtUtc)
                .Select(x => new CompanyProofOfDeliveryItemViewModel
                {
                    DeliveredAtUtc = x.DeliveredAtUtc,
                    ReceiverName = x.ReceiverName,
                    Notes = x.Notes
                })
                .ToListAsync();

            var packages = await _dbContext.Packages
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .Include(x => x.Items)
                .OrderBy(x => x.PackageNo)
                .Select(x => new CompanyPackageItemViewModel
                {
                    PackageNo = x.PackageNo,
                    PackageType = x.PackageType.ToString(),
                    WeightKg = x.WeightKg,
                    VolumeCbm = x.VolumeCbm,
                    ItemsCount = x.Items.Count
                })
                .ToListAsync();

            var cargoItems = await _dbContext.CargoItems
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .OrderBy(x => x.Description)
                .Select(x => new CompanyCargoItemItemViewModel
                {
                    Description = x.Description,
                    CargoItemType = x.CargoItemType.ToString(),
                    Quantity = x.Quantity,
                    UnitOfMeasure = x.UnitOfMeasure,
                    GrossWeightKg = x.GrossWeightKg,
                    VolumeCbm = x.VolumeCbm,
                    IsStackable = x.IsStackable,
                    IsFragile = x.IsFragile
                })
                .ToListAsync();

            var references = await _dbContext.ShipmentReferences
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .OrderBy(x => x.ReferenceType)
                .Select(x => new CompanyShipmentReferenceItemViewModel
                {
                    ReferenceType = x.ReferenceType.ToString(),
                    ReferenceValue = x.ReferenceValue,
                    Description = x.Description
                })
                .ToListAsync();

            var tags = await _dbContext.ShipmentTags
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .OrderBy(x => x.TagType)
                .Select(x => new CompanyShipmentTagItemViewModel
                {
                    TagType = x.TagType.ToString(),
                    CustomValue = x.CustomValue,
                    Notes = x.Notes
                })
                .ToListAsync();

            var voyages = await _dbContext.ShipmentVoyages
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .Include(x => x.Voyage)
                .Select(x => new CompanyShipmentVoyageItemViewModel
                {
                    VoyageNo = x.Voyage.VoyageNumber,
                    BookingReference = x.BookingReference,
                    ShipmentLegId = x.ShipmentLegId
                })
                .ToListAsync();

            var trips = await _dbContext.ShipmentTrips
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .Include(x => x.Trip)
                .Select(x => new CompanyShipmentTripItemViewModel
                {
                    TripNo = x.Trip.TripNo,
                    ShipmentLegId = x.ShipmentLegId,
                    Notes = x.Notes
                })
                .ToListAsync();

            var containers = await _dbContext.ShipmentContainers
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .Include(x => x.Container)
                .Select(x => new CompanyShipmentContainerItemViewModel
                {
                    ContainerNo = x.Container.ContainerNumber,
                    ShipmentLegId = x.ShipmentLegId,
                    GrossWeightKg = x.GrossWeightKg,
                    SealNumber = x.SealNumber
                })
                .ToListAsync();

            var ulds = await _dbContext.ShipmentULDs
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .Include(x => x.Uld)
                .Select(x => new CompanyShipmentUldItemViewModel
                {
                    UldCode = x.Uld.UldNumber,
                    ShipmentLegId = x.ShipmentLegId,
                    GrossWeightKg = x.GrossWeightKg,
                    VolumeCbm = x.VolumeCbm
                })
                .ToListAsync();

            return new CompanyShipmentDetailsViewModel
            {
                Id = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                Status = shipment.Status.ToString(),
                PrimaryMode = shipment.PrimaryMode.ToString(),
                Incoterm = shipment.Incoterm?.ToString(),
                DeclaredValue = shipment.DeclaredValue,
                Currency = shipment.Currency,
                CustomerReference = shipment.CustomerReference,
                Notes = shipment.Notes,
                OrderNo = shipment.Order?.OrderNo,
                SenderAddress = shipment.SenderAddress != null
                    ? $"{shipment.SenderAddress.Street}, {shipment.SenderAddress.City}, {shipment.SenderAddress.Country}"
                    : null,
                ReceiverAddress = shipment.ReceiverAddress != null
                    ? $"{shipment.ReceiverAddress.Street}, {shipment.ReceiverAddress.City}, {shipment.ReceiverAddress.Country}"
                    : null,
                CanEditStatus = CanChangeShipmentStatus(shipment.Status),
                Parties = shipment.Parties
                    .OrderBy(x => x.Role)
                    .Select(x => new CompanyShipmentPartyItemViewModel
                    {
                        CompanyName = x.Company.Name,
                        Role = x.Role.ToString()
                    })
                    .ToList(),
                Legs = shipment.Legs
                    .OrderBy(x => x.LegNo)
                    .Select(x => new CompanyShipmentLegItemViewModel
                    {
                        Id = x.Id,
                        LegNo = x.LegNo,
                        Mode = x.Mode.ToString(),
                        Origin = x.OriginLocation.Name,
                        Destination = x.DestinationLocation.Name,
                        Status = x.Status.ToString(),
                        ETD_Utc = x.ETD_Utc,
                        ETA_Utc = x.ETA_Utc,
                        ATD_Utc = x.ATD_Utc,
                        ATA_Utc = x.ATA_Utc,
                        CarrierReference = x.CarrierReference
                    })
                    .ToList(),
                StatusHistories = statusHistory,
                TrackingEvents = trackingEvents,
                ProofOfDeliveries = pods,
                Packages = packages,
                CargoItems = cargoItems,
                References = references,
                Tags = tags,
                Voyages = voyages,
                Trips = trips,
                Containers = containers,
                Ulds = ulds
            };
        }
        public async Task<bool> UpdateShipmentStatusAsync(Guid companyId, Guid shipmentId, string newStatus, string? reason = null)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .FirstOrDefaultAsync(x => x.Id == shipmentId);

            if (shipment == null)
            {
                return false;
            }

            if (!Enum.TryParse<ShipmentStatus>(newStatus, true, out var parsedStatus))
            {
                return false;
            }

            if (!CanTransitionShipmentStatus(shipment.Status, parsedStatus))
            {
                return false;
            }

            var oldStatus = shipment.Status;
            shipment.Status = parsedStatus;
            shipment.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.ShipmentStatusHistories.AddAsync(new Domain.Shipments.ShipmentStatusHistory
            {
                ShipmentId = shipment.Id,
                OldStatus = oldStatus,
                NewStatus = parsedStatus,
                ChangedAtUtc = DateTime.UtcNow,
                Reason = string.IsNullOrWhiteSpace(reason)
            ? $"Status changed by company from {oldStatus} to {parsedStatus}."
            : reason
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        // Shipment Parties

        #region Shipment Parties

        public async Task<CompanyShipmentPartiesViewModel?> GetShipmentPartiesAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    CanManage = x.CustomerCompanyId == companyId
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var parties = await _dbContext.ShipmentParties
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .Select(x => new CompanyShipmentPartyListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    CompanyId = x.CompanyId,
                    CompanyName = x.Company.Name,
                    Role = x.Role.ToString(),
                    CompanyContactId = x.CompanyContactId,
                    CompanyContactName = x.CompanyContactId != null
                        ? (
                            _dbContext.CompanyContacts
                                .Where(c => c.Id == x.CompanyContactId && !c.IsDeleted)
                                .Select(c => (c.FullName).Trim())
                                .FirstOrDefault()
                          )
                        : null
                })
                .OrderBy(x => x.Role)
                .ThenBy(x => x.CompanyName)
                .ToListAsync();

            return new CompanyShipmentPartiesViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CanManageParties = shipment.CanManage,
                Parties = parties
            };
        }
        public async Task<CompanyShipmentPartyCreateViewModel?> GetCreateShipmentPartyModelAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            x.Id == shipmentId &&
                            x.CustomerCompanyId == companyId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var model = new CompanyShipmentPartyCreateViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo
            };

            await PopulateShipmentPartyCreateOptionsAsync(model);
            return model;
        }
        public async Task<CompanyShipmentPartyCreateResult> CreateShipmentPartyAsync(Guid companyId, CompanyShipmentPartyCreateViewModel model)
        {
            var canManage = await CanManageShipmentAsync(companyId, model.ShipmentId);
            if (!canManage)
            {
                return new CompanyShipmentPartyCreateResult
                {
                    Succeeded = false,
                    ErrorMessage = "Нямате право да добавяте parties към този shipment."
                };
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId);

            if (!shipmentExists)
            {
                return new CompanyShipmentPartyCreateResult
                {
                    Succeeded = false,
                    ErrorMessage = "Shipment-ът не беше намерен."
                };
            }

            var companyExists = await _dbContext.Companies
                .AnyAsync(x => !x.IsDeleted && x.Id == model.CompanyId);

            if (!companyExists)
            {
                return new CompanyShipmentPartyCreateResult
                {
                    Succeeded = false,
                    ErrorMessage = "Избраната компания не съществува."
                };
            }

            var alreadyExists = await _dbContext.ShipmentParties
                .AnyAsync(x => !x.IsDeleted &&
                               x.ShipmentId == model.ShipmentId &&
                               x.CompanyId == model.CompanyId &&
                               x.Role == model.Role);

            if (alreadyExists)
            {
                return new CompanyShipmentPartyCreateResult
                {
                    Succeeded = false,
                    ErrorMessage = "Вече съществува shipment party със същата компания и роля."
                };
            }

            if (model.CompanyContactId.HasValue)
            {
                var contactExists = await _dbContext.CompanyContacts
                    .AnyAsync(x => !x.IsDeleted &&
                                   x.Id == model.CompanyContactId.Value &&
                                   x.CompanyId == model.CompanyId);

                if (!contactExists)
                {
                    return new CompanyShipmentPartyCreateResult
                    {
                        Succeeded = false,
                        ErrorMessage = "Избраният контакт не принадлежи на избраната компания."
                    };
                }
            }

            var entity = new ShipmentParty
            {
                ShipmentId = model.ShipmentId,
                CompanyId = model.CompanyId,
                Role = model.Role,
                CompanyContactId = model.CompanyContactId,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _dbContext.ShipmentParties.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return new CompanyShipmentPartyCreateResult
            {
                Succeeded = true
            };
        }
        public async Task<CompanyShipmentPartyEditViewModel?> GetShipmentPartyForEditAsync(Guid companyId, Guid shipmentPartyId)
        {
            var entity = await _dbContext.ShipmentParties
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            x.Id == shipmentPartyId &&
                            x.Shipment.CustomerCompanyId == companyId)
                .Select(x => new CompanyShipmentPartyEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    CompanyId = x.CompanyId,
                    Role = x.Role,
                    CompanyContactId = x.CompanyContactId
                })
                .FirstOrDefaultAsync();

            if (entity == null)
            {
                return null;
            }

            await PopulateShipmentPartyEditOptionsAsync(entity);
            return entity;
        }
        public async Task<bool> UpdateShipmentPartyAsync(Guid companyId, CompanyShipmentPartyEditViewModel model)
        {
            var entity = await _dbContext.ShipmentParties
                .FirstOrDefaultAsync(x => !x.IsDeleted &&
                                          x.Id == model.Id &&
                                          x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            var duplicateExists = await _dbContext.ShipmentParties
                .AnyAsync(x => !x.IsDeleted &&
                               x.Id != model.Id &&
                               x.ShipmentId == model.ShipmentId &&
                               x.CompanyId == model.CompanyId &&
                               x.Role == model.Role);

            if (duplicateExists)
            {
                return false;
            }

            entity.CompanyId = model.CompanyId;
            entity.Role = model.Role;
            entity.CompanyContactId = model.CompanyContactId;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteShipmentPartyAsync(Guid companyId, Guid shipmentPartyId)
        {
            var entity = await _dbContext.ShipmentParties
                .FirstOrDefaultAsync(x => !x.IsDeleted &&
                                          x.Id == shipmentPartyId &&
                                          x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        // Shipment Legs

        #region Shipment Legs

        public async Task<CompanyShipmentLegsViewModel?> GetShipmentLegsAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    CanManage = x.CustomerCompanyId == companyId
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var legs = await _dbContext.ShipmentLegs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderBy(x => x.LegNo)
                .Select(x => new CompanyShipmentLegListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    LegNo = x.LegNo,
                    Mode = x.Mode.ToString(),
                    OriginLocation = x.OriginLocation.Name,
                    DestinationLocation = x.DestinationLocation.Name,
                    Status = x.Status.ToString(),
                    ETD_Utc = x.ETD_Utc,
                    ETA_Utc = x.ETA_Utc,
                    ATD_Utc = x.ATD_Utc,
                    ATA_Utc = x.ATA_Utc,
                    CarrierReference = x.CarrierReference,
                    Notes = x.Notes,
                    StatusHistoryCount = _dbContext.LegStatusHistories
                        .Count(h => !h.IsDeleted && h.ShipmentLegId == x.Id)
                })
                .ToListAsync();

            return new CompanyShipmentLegsViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CanManageLegs = shipment.CanManage,
                Legs = legs
            };
        }
        public async Task<CompanyShipmentLegCreateViewModel?> GetCreateShipmentLegModelAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            x.Id == shipmentId &&
                            x.CustomerCompanyId == companyId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var model = new CompanyShipmentLegCreateViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                LegNo = await GetNextShipmentLegNoAsync(shipmentId)
            };

            await PopulateShipmentLegCreateOptionsAsync(model);
            return model;
        }
        public async Task<Guid?> CreateShipmentLegAsync(Guid companyId, CompanyShipmentLegCreateViewModel model)
        {
            var canManage = await CanManageShipmentAsync(companyId, model.ShipmentId);
            if (!canManage)
            {
                return null;
            }

            var duplicateLegNo = await _dbContext.ShipmentLegs
                .AnyAsync(x => !x.IsDeleted &&
                               x.ShipmentId == model.ShipmentId &&
                               x.LegNo == model.LegNo);

            if (duplicateLegNo)
            {
                return null;
            }

            var entity = new ShipmentLeg
            {
                ShipmentId = model.ShipmentId,
                LegNo = model.LegNo,
                Mode = model.Mode,
                OriginLocationId = model.OriginLocationId,
                DestinationLocationId = model.DestinationLocationId,
                Status = model.Status,
                ETD_Utc = model.ETD_Utc,
                ETA_Utc = model.ETA_Utc,
                ATD_Utc = model.ATD_Utc,
                ATA_Utc = model.ATA_Utc,
                CarrierReference = string.IsNullOrWhiteSpace(model.CarrierReference)
                    ? null
                    : model.CarrierReference.Trim(),
                Notes = string.IsNullOrWhiteSpace(model.Notes)
                    ? null
                    : model.Notes.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _dbContext.ShipmentLegs.AddAsync(entity);

            await _dbContext.LegStatusHistories.AddAsync(new LegStatusHistory
            {
                ShipmentLegId = entity.Id,
                OldStatus = entity.Status,
                NewStatus = entity.Status,
                ChangedAtUtc = DateTime.UtcNow,
                Reason = "Initial leg creation from company portal."
            });

            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }
        public async Task<CompanyShipmentLegEditViewModel?> GetShipmentLegForEditAsync(Guid companyId, Guid shipmentLegId)
        {
            var model = await _dbContext.ShipmentLegs
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            x.Id == shipmentLegId &&
                            x.Shipment.CustomerCompanyId == companyId)
                .Select(x => new CompanyShipmentLegEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    LegNo = x.LegNo,
                    Mode = x.Mode,
                    OriginLocationId = x.OriginLocationId,
                    DestinationLocationId = x.DestinationLocationId,
                    CurrentStatus = x.Status.ToString(),
                    ETD_Utc = x.ETD_Utc,
                    ETA_Utc = x.ETA_Utc,
                    ATD_Utc = x.ATD_Utc,
                    ATA_Utc = x.ATA_Utc,
                    CarrierReference = x.CarrierReference,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            await PopulateShipmentLegEditOptionsAsync(model);
            return model;
        }
        public async Task<bool> UpdateShipmentLegAsync(Guid companyId, CompanyShipmentLegEditViewModel model)
        {
            var entity = await _dbContext.ShipmentLegs
                .FirstOrDefaultAsync(x => !x.IsDeleted &&
                                          x.Id == model.Id &&
                                          x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            var duplicateLegNo = await _dbContext.ShipmentLegs
                .AnyAsync(x => !x.IsDeleted &&
                               x.Id != model.Id &&
                               x.ShipmentId == model.ShipmentId &&
                               x.LegNo == model.LegNo);

            if (duplicateLegNo)
            {
                return false;
            }

            entity.LegNo = model.LegNo;
            entity.Mode = model.Mode;
            entity.OriginLocationId = model.OriginLocationId;
            entity.DestinationLocationId = model.DestinationLocationId;
            entity.ETD_Utc = model.ETD_Utc;
            entity.ETA_Utc = model.ETA_Utc;
            entity.ATD_Utc = model.ATD_Utc;
            entity.ATA_Utc = model.ATA_Utc;
            entity.CarrierReference = string.IsNullOrWhiteSpace(model.CarrierReference)
                ? null
                : model.CarrierReference.Trim();
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes)
                ? null
                : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteShipmentLegAsync(Guid companyId, Guid shipmentLegId)
        {
            var entity = await _dbContext.ShipmentLegs
                .FirstOrDefaultAsync(x => !x.IsDeleted &&
                                          x.Id == shipmentLegId &&
                                          x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<CompanyShipmentLegDetailsViewModel?> GetShipmentLegDetailsAsync(Guid companyId, Guid shipmentLegId)
        {
            var model = await _dbContext.ShipmentLegs
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            x.Id == shipmentLegId &&
                            (x.Shipment.CustomerCompanyId == companyId ||
                             x.Shipment.Parties.Any(p => p.CompanyId == companyId)))
                .Select(x => new CompanyShipmentLegDetailsViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    LegNo = x.LegNo,
                    Mode = x.Mode.ToString(),
                    OriginLocation = x.OriginLocation.Name,
                    DestinationLocation = x.DestinationLocation.Name,
                    Status = x.Status.ToString(),
                    ETD_Utc = x.ETD_Utc,
                    ETA_Utc = x.ETA_Utc,
                    ATD_Utc = x.ATD_Utc,
                    ATA_Utc = x.ATA_Utc,
                    CarrierReference = x.CarrierReference,
                    Notes = x.Notes,
                    CanManageLeg = x.Shipment.CustomerCompanyId == companyId,
                    StatusHistories = _dbContext.LegStatusHistories
                        .Where(h => !h.IsDeleted && h.ShipmentLegId == x.Id)
                        .OrderByDescending(h => h.ChangedAtUtc)
                        .Select(h => new CompanyLegStatusHistoryItemViewModel
                        {
                            Id = h.Id,
                            OldStatus = h.OldStatus.ToString(),
                            NewStatus = h.NewStatus.ToString(),
                            ChangedAtUtc = h.ChangedAtUtc,
                            Reason = h.Reason
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return model;
        }
        public async Task<bool> UpdateShipmentLegStatusAsync(Guid companyId, CompanyShipmentLegStatusUpdateViewModel model)
        {
            var entity = await _dbContext.ShipmentLegs
                .FirstOrDefaultAsync(x => !x.IsDeleted &&
                                          x.Id == model.ShipmentLegId &&
                                          x.ShipmentId == model.ShipmentId &&
                                          x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            if (entity.Status == model.NewStatus)
            {
                return true;
            }

            var oldStatus = entity.Status;
            entity.Status = model.NewStatus;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.LegStatusHistories.AddAsync(new LegStatusHistory
            {
                ShipmentLegId = entity.Id,
                OldStatus = oldStatus,
                NewStatus = model.NewStatus,
                ChangedAtUtc = DateTime.UtcNow,
                Reason = string.IsNullOrWhiteSpace(model.Reason) ? null : model.Reason.Trim()
            });

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        // Tracking Events

        #region Tracking Events

        public async Task<CompanyTrackingEventsViewModel?> GetTrackingEventsAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    CanManage = x.CustomerCompanyId == companyId
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var events = await _dbContext.TrackingEvents
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderByDescending(x => x.EventTimeUtc)
                .Select(x => new CompanyTrackingEventListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    EventType = x.EventType.ToString(),
                    EventTimeUtc = x.EventTimeUtc,
                    LocationId = x.LocationId,
                    LocationName = x.Location != null ? x.Location.Name : null,
                    Details = x.Details,
                    Source = x.Source
                })
                .ToListAsync();

            return new CompanyTrackingEventsViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CanManageTrackingEvents = shipment.CanManage,
                TrackingEvents = events
            };
        }
        public async Task<CompanyTrackingEventCreateViewModel?> GetCreateTrackingEventModelAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            x.Id == shipmentId &&
                            x.CustomerCompanyId == companyId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var model = new CompanyTrackingEventCreateViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                EventTimeUtc = DateTime.UtcNow
            };

            await PopulateTrackingEventCreateOptionsAsync(model);
            return model;
        }
        public async Task<Guid?> CreateTrackingEventAsync(Guid companyId, CompanyTrackingEventCreateViewModel model)
        {
            var canManage = await CanManageShipmentAsync(companyId, model.ShipmentId);
            if (!canManage)
            {
                return null;
            }

            var entity = new TrackingEvent
            {
                ShipmentId = model.ShipmentId,
                EventType = model.EventType,
                EventTimeUtc = model.EventTimeUtc,
                LocationId = model.LocationId,
                Details = string.IsNullOrWhiteSpace(model.Details) ? null : model.Details.Trim(),
                Source = string.IsNullOrWhiteSpace(model.Source) ? null : model.Source.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };

            await _dbContext.TrackingEvents.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }
        public async Task<CompanyTrackingEventEditViewModel?> GetTrackingEventForEditAsync(Guid companyId, Guid trackingEventId)
        {
            var model = await _dbContext.TrackingEvents
                .AsNoTracking()
                .Where(x => !x.IsDeleted &&
                            x.Id == trackingEventId &&
                            x.Shipment.CustomerCompanyId == companyId)
                .Select(x => new CompanyTrackingEventEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    EventType = x.EventType,
                    EventTimeUtc = x.EventTimeUtc,
                    LocationId = x.LocationId,
                    Details = x.Details,
                    Source = x.Source
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            await PopulateTrackingEventEditOptionsAsync(model);
            return model;
        }
        public async Task<bool> UpdateTrackingEventAsync(Guid companyId, CompanyTrackingEventEditViewModel model)
        {
            var entity = await _dbContext.TrackingEvents
                .FirstOrDefaultAsync(x => !x.IsDeleted &&
                                          x.Id == model.Id &&
                                          x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.EventType = model.EventType;
            entity.EventTimeUtc = model.EventTimeUtc;
            entity.LocationId = model.LocationId;
            entity.Details = string.IsNullOrWhiteSpace(model.Details) ? null : model.Details.Trim();
            entity.Source = string.IsNullOrWhiteSpace(model.Source) ? null : model.Source.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteTrackingEventAsync(Guid companyId, Guid trackingEventId)
        {
            var entity = await _dbContext.TrackingEvents
                .FirstOrDefaultAsync(x => !x.IsDeleted &&
                                          x.Id == trackingEventId &&
                                          x.Shipment.CustomerCompanyId == companyId);

            if (entity == null)
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        // PROOF OF DELIVERIES

        #region Proof of Deliveries

        public async Task<CompanyProofOfDeliveriesViewModel?> GetProofOfDeliveriesAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    CanManage = x.CustomerCompanyId == companyId
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var pods = await _dbContext.ProofOfDeliveries
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderByDescending(x => x.DeliveredAtUtc)
                .Select(x => new CompanyProofOfDeliveryListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    DeliveredAtUtc = x.DeliveredAtUtc,
                    ReceiverName = x.ReceiverName,
                    Notes = x.Notes,
                    SignatureFileResourceId = x.SignatureFileResourceId
                })
                .ToListAsync();

            var hasExistingProofOfDelivery = pods.Any();

            return new CompanyProofOfDeliveriesViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CanManageProofOfDeliveries = shipment.CanManage,
                CanCreateProofOfDelivery = shipment.CanManage && !hasExistingProofOfDelivery,
                ProofOfDeliveries = pods
            };
        }

        public async Task<CompanyProofOfDeliveryCreateViewModel?> GetCreateProofOfDeliveryModelAsync(Guid companyId, Guid shipmentId)
        {
            if (!await CanManageShipmentAsync(companyId, shipmentId))
            {
                return null;
            }

            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var existingPodExists = await _dbContext.ProofOfDeliveries
                .AnyAsync(x => !x.IsDeleted && x.ShipmentId == shipmentId);

            if (existingPodExists)
            {
                return null;
            }

            return new CompanyProofOfDeliveryCreateViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                DeliveredAtUtc = DateTime.UtcNow
            };
        }

        public async Task<Guid?> CreateProofOfDeliveryAsync(Guid companyId, CompanyProofOfDeliveryCreateViewModel model)
        {
            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId);

            if (!shipmentExists)
            {
                return null;
            }

            var existingPodExists = await _dbContext.ProofOfDeliveries
                .AnyAsync(x => !x.IsDeleted && x.ShipmentId == model.ShipmentId);

            if (existingPodExists)
            {
                return null;
            }

            var entity = new ProofOfDelivery
            {
                ShipmentId = model.ShipmentId,
                DeliveredAtUtc = DateTime.SpecifyKind(model.DeliveredAtUtc, DateTimeKind.Utc),
                ReceiverName = string.IsNullOrWhiteSpace(model.ReceiverName) ? null : model.ReceiverName.Trim(),
                SignatureFileResourceId = model.SignatureFileResourceId,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            await _dbContext.ProofOfDeliveries.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<CompanyProofOfDeliveryEditViewModel?> GetProofOfDeliveryForEditAsync(Guid companyId, Guid proofOfDeliveryId)
        {
            var model = await _dbContext.ProofOfDeliveries
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == proofOfDeliveryId)
                .Select(x => new CompanyProofOfDeliveryEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    DeliveredAtUtc = x.DeliveredAtUtc,
                    ReceiverName = x.ReceiverName,
                    SignatureFileResourceId = x.SignatureFileResourceId,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            return model;
        }

        public async Task<bool> UpdateProofOfDeliveryAsync(Guid companyId, CompanyProofOfDeliveryEditViewModel model)
        {
            var entity = await _dbContext.ProofOfDeliveries
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            entity.DeliveredAtUtc = DateTime.SpecifyKind(model.DeliveredAtUtc, DateTimeKind.Utc);
            entity.ReceiverName = string.IsNullOrWhiteSpace(model.ReceiverName) ? null : model.ReceiverName.Trim();
            entity.SignatureFileResourceId = model.SignatureFileResourceId;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #region Delete Proof of Delivery
        //public async Task<bool> DeleteProofOfDeliveryAsync(Guid companyId, Guid proofOfDeliveryId)
        //{
        //    var entity = await _dbContext.ProofOfDeliveries
        //        .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == proofOfDeliveryId);

        //    if (entity == null)
        //    {
        //        return false;
        //    }

        //    if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
        //    {
        //        return false;
        //    }

        //    entity.IsDeleted = true;
        //    entity.DeletedAtUtc = DateTime.UtcNow;

        //    await _dbContext.SaveChangesAsync();
        //    return true;
        //}
        #endregion

        #endregion

        // PACKAGES

        #region Packages

        public async Task<CompanyPackagesViewModel?> GetPackagesAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    CanManage = x.CustomerCompanyId == companyId
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var packages = await _dbContext.Packages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderBy(x => x.PackageNo)
                .Select(x => new CompanyPackageListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    PackageNo = x.PackageNo,
                    PackageType = x.PackageType.ToString(),
                    WeightKg = x.WeightKg,
                    LengthCm = x.LengthCm,
                    WidthCm = x.WidthCm,
                    HeightCm = x.HeightCm,
                    VolumeCbm = x.VolumeCbm,
                    Notes = x.Notes,
                    ItemsCount = x.Items.Count(i => !i.IsDeleted)
                })
                .ToListAsync();

            return new CompanyPackagesViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CanManagePackages = shipment.CanManage,
                Packages = packages
            };
        }

        public async Task<CompanyPackageCreateViewModel?> GetCreatePackageModelAsync(Guid companyId, Guid shipmentId)
        {
            if (!await CanManageShipmentAsync(companyId, shipmentId))
            {
                return null;
            }

            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            return new CompanyPackageCreateViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo
            };
        }

        public async Task<Guid?> CreatePackageAsync(Guid companyId, CompanyPackageCreateViewModel model)
        {
            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId);

            if (!shipmentExists)
            {
                return null;
            }

            var duplicatePackageNo = await _dbContext.Packages
                .AnyAsync(x => !x.IsDeleted
                               && x.ShipmentId == model.ShipmentId
                               && x.PackageNo == model.PackageNo);

            if (duplicatePackageNo)
            {
                return null;
            }

            var entity = new Package
            {
                ShipmentId = model.ShipmentId,
                PackageNo = model.PackageNo.Trim(),
                PackageType = model.PackageType,
                WeightKg = model.WeightKg,
                LengthCm = model.LengthCm,
                WidthCm = model.WidthCm,
                HeightCm = model.HeightCm,
                VolumeCbm = model.VolumeCbm,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            await _dbContext.Packages.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<CompanyPackageEditViewModel?> GetPackageForEditAsync(Guid companyId, Guid packageId)
        {
            var model = await _dbContext.Packages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == packageId)
                .Select(x => new CompanyPackageEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    PackageNo = x.PackageNo,
                    PackageType = x.PackageType,
                    WeightKg = x.WeightKg,
                    LengthCm = x.LengthCm,
                    WidthCm = x.WidthCm,
                    HeightCm = x.HeightCm,
                    VolumeCbm = x.VolumeCbm,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            return model;
        }

        public async Task<bool> UpdatePackageAsync(Guid companyId, CompanyPackageEditViewModel model)
        {
            var entity = await _dbContext.Packages
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            var duplicatePackageNo = await _dbContext.Packages
                .AnyAsync(x => !x.IsDeleted
                               && x.ShipmentId == entity.ShipmentId
                               && x.Id != entity.Id
                               && x.PackageNo == model.PackageNo);

            if (duplicatePackageNo)
            {
                return false;
            }

            entity.PackageNo = model.PackageNo.Trim();
            entity.PackageType = model.PackageType;
            entity.WeightKg = model.WeightKg;
            entity.LengthCm = model.LengthCm;
            entity.WidthCm = model.WidthCm;
            entity.HeightCm = model.HeightCm;
            entity.VolumeCbm = model.VolumeCbm;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePackageAsync(Guid companyId, Guid packageId)
        {
            var entity = await _dbContext.Packages
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == packageId);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            foreach (var item in entity.Items.Where(x => !x.IsDeleted))
            {
                item.IsDeleted = true;
                item.DeletedAtUtc = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }


        public async Task<Guid?> CreateAgreementAsync(Guid companyId, AgreementCreateViewModel model)
        {
            var duplicate = await _dbContext.Agreements.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.AgreementNumber == model.AgreementNumber);
            if (duplicate) return null;

            var entity = new Agreement
            {
                CompanyId = companyId,
                AgreementNumber = model.AgreementNumber.Trim(),
                AgreementType = model.AgreementType,
                ValidFromUtc = model.ValidFromUtc,
                ValidToUtc = model.ValidToUtc,
                Currency = model.Currency.Trim().ToUpperInvariant(),
                PaymentTerms = string.IsNullOrWhiteSpace(model.PaymentTerms) ? null : model.PaymentTerms.Trim(),
                IsActive = model.IsActive,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            await _dbContext.Agreements.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<AgreementEditViewModel?> GetAgreementForEditAsync(Guid companyId, Guid agreementId)
        {
            return await _dbContext.Agreements
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == agreementId)
                .Select(x => new AgreementEditViewModel
                {
                    Id = x.Id,
                    AgreementNumber = x.AgreementNumber,
                    AgreementType = x.AgreementType,
                    ValidFromUtc = x.ValidFromUtc,
                    ValidToUtc = x.ValidToUtc,
                    Currency = x.Currency,
                    PaymentTerms = x.PaymentTerms,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateAgreementAsync(Guid companyId, AgreementEditViewModel model)
        {
            var entity = await _dbContext.Agreements.FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.Id);
            if (entity == null) return false;

            var duplicate = await _dbContext.Agreements.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id != model.Id && x.AgreementNumber == model.AgreementNumber);
            if (duplicate) return false;

            entity.AgreementNumber = model.AgreementNumber.Trim();
            entity.AgreementType = model.AgreementType;
            entity.ValidFromUtc = model.ValidFromUtc;
            entity.ValidToUtc = model.ValidToUtc;
            entity.Currency = model.Currency.Trim().ToUpperInvariant();
            entity.PaymentTerms = string.IsNullOrWhiteSpace(model.PaymentTerms) ? null : model.PaymentTerms.Trim();
            entity.IsActive = model.IsActive;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAgreementAsync(Guid companyId, Guid agreementId)
        {
            var entity = await _dbContext.Agreements
                .Include(x => x.DiscountRules)
                .Include(x => x.PricingQuotes)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == agreementId);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            foreach (var item in entity.DiscountRules.Where(x => !x.IsDeleted))
            {
                item.IsDeleted = true;
                item.DeletedAtUtc = DateTime.UtcNow;
            }
            foreach (var item in entity.PricingQuotes.Where(x => !x.IsDeleted))
            {
                item.AgreementId = null;
                item.UpdatedAtUtc = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<DiscountRuleCreateViewModel> GetCreateDiscountRuleModelAsync(Guid companyId)
        {
            var model = new DiscountRuleCreateViewModel();
            await PopulateDiscountRuleOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreateDiscountRuleAsync(Guid companyId, DiscountRuleCreateViewModel model)
        {
            if (!await _dbContext.Agreements.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.AgreementId)) return null;
            if (model.ServiceLevelId.HasValue && !await _dbContext.ServiceLevels.AnyAsync(x => !x.IsDeleted && x.Id == model.ServiceLevelId.Value)) return null;
            if (model.GeoZoneId.HasValue && !await _dbContext.GeoZones.AnyAsync(x => !x.IsDeleted && x.Id == model.GeoZoneId.Value)) return null;

            var entity = new DiscountRule
            {
                AgreementId = model.AgreementId,
                ServiceLevelId = model.ServiceLevelId,
                GeoZoneId = model.GeoZoneId,
                DiscountType = model.DiscountType,
                Value = model.Value,
                MinShipmentValue = model.MinShipmentValue,
                MaxShipmentValue = model.MaxShipmentValue,
                ValidFromUtc = model.ValidFromUtc,
                ValidToUtc = model.ValidToUtc,
                IsActive = model.IsActive,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };
            await _dbContext.DiscountRules.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<DiscountRuleEditViewModel?> GetDiscountRuleForEditAsync(Guid companyId, Guid discountRuleId)
        {
            var model = await _dbContext.DiscountRules
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == discountRuleId && x.Agreement.CompanyId == companyId)
                .Select(x => new DiscountRuleEditViewModel
                {
                    Id = x.Id,
                    AgreementId = x.AgreementId,
                    ServiceLevelId = x.ServiceLevelId,
                    GeoZoneId = x.GeoZoneId,
                    DiscountType = x.DiscountType,
                    Value = x.Value,
                    MinShipmentValue = x.MinShipmentValue,
                    MaxShipmentValue = x.MaxShipmentValue,
                    ValidFromUtc = x.ValidFromUtc,
                    ValidToUtc = x.ValidToUtc,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                }).FirstOrDefaultAsync();
            if (model == null) return null;
            await PopulateDiscountRuleOptionsAsync(companyId, model);
            return model;
        }

        public async Task<bool> UpdateDiscountRuleAsync(Guid companyId, DiscountRuleEditViewModel model)
        {
            var entity = await _dbContext.DiscountRules.Include(x => x.Agreement).FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);
            if (entity == null || entity.Agreement.CompanyId != companyId) return false;
            if (!await _dbContext.Agreements.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.AgreementId)) return false;

            entity.AgreementId = model.AgreementId;
            entity.ServiceLevelId = model.ServiceLevelId;
            entity.GeoZoneId = model.GeoZoneId;
            entity.DiscountType = model.DiscountType;
            entity.Value = model.Value;
            entity.MinShipmentValue = model.MinShipmentValue;
            entity.MaxShipmentValue = model.MaxShipmentValue;
            entity.ValidFromUtc = model.ValidFromUtc;
            entity.ValidToUtc = model.ValidToUtc;
            entity.IsActive = model.IsActive;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDiscountRuleAsync(Guid companyId, Guid discountRuleId)
        {
            var entity = await _dbContext.DiscountRules.Include(x => x.Agreement).FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == discountRuleId);
            if (entity == null || entity.Agreement.CompanyId != companyId) return false;
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<PricingQuoteCreateViewModel> GetCreatePricingQuoteModelAsync(Guid companyId)
        {
            var model = new PricingQuoteCreateViewModel();
            await PopulatePricingQuoteOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreatePricingQuoteAsync(Guid companyId, PricingQuoteCreateViewModel model)
        {
            if (await _dbContext.PricingQuotes.AnyAsync(x => !x.IsDeleted && x.CustomerCompanyId == companyId && x.QuoteNumber == model.QuoteNumber)) return null;
            if (model.AgreementId.HasValue && !await _dbContext.Agreements.AnyAsync(x => !x.IsDeleted && x.CompanyId == companyId && x.Id == model.AgreementId.Value)) return null;
            if (model.OrderId.HasValue && !await _dbContext.Orders.AnyAsync(x => !x.IsDeleted && x.CustomerCompanyId == companyId && x.Id == model.OrderId.Value)) return null;
            if (model.ShipmentId.HasValue && !await _dbContext.Shipments.AnyAsync(x => !x.IsDeleted && x.CustomerCompanyId == companyId && x.Id == model.ShipmentId.Value)) return null;
            if (model.ServiceLevelId.HasValue && !await _dbContext.ServiceLevels.AnyAsync(x => !x.IsDeleted && x.Id == model.ServiceLevelId.Value)) return null;

            var entity = new PricingQuote
            {
                QuoteNumber = model.QuoteNumber.Trim(),
                CustomerCompanyId = companyId,
                AgreementId = model.AgreementId,
                OrderId = model.OrderId,
                ShipmentId = model.ShipmentId,
                ServiceLevelId = model.ServiceLevelId,
                Status = model.Status,
                Currency = model.Currency.Trim().ToUpperInvariant(),
                ValidUntilUtc = model.ValidUntilUtc,
                NetAmount = model.NetAmount,
                TaxAmount = model.TaxAmount,
                TotalAmount = model.TotalAmount,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };
            await _dbContext.PricingQuotes.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<PricingQuoteEditViewModel?> GetPricingQuoteForEditAsync(Guid companyId, Guid pricingQuoteId)
        {
            var model = await _dbContext.PricingQuotes.AsNoTracking()
                .Where(x => !x.IsDeleted && x.CustomerCompanyId == companyId && x.Id == pricingQuoteId)
                .Select(x => new PricingQuoteEditViewModel
                {
                    Id = x.Id,
                    QuoteNumber = x.QuoteNumber,
                    AgreementId = x.AgreementId,
                    OrderId = x.OrderId,
                    ShipmentId = x.ShipmentId,
                    ServiceLevelId = x.ServiceLevelId,
                    Status = x.Status,
                    Currency = x.Currency,
                    ValidUntilUtc = x.ValidUntilUtc,
                    NetAmount = x.NetAmount,
                    TaxAmount = x.TaxAmount,
                    TotalAmount = x.TotalAmount,
                    Notes = x.Notes
                }).FirstOrDefaultAsync();
            if (model == null) return null;
            await PopulatePricingQuoteOptionsAsync(companyId, model);
            return model;
        }

        public async Task<bool> UpdatePricingQuoteAsync(Guid companyId, PricingQuoteEditViewModel model)
        {
            var entity = await _dbContext.PricingQuotes.FirstOrDefaultAsync(x => !x.IsDeleted && x.CustomerCompanyId == companyId && x.Id == model.Id);
            if (entity == null) return false;
            if (await _dbContext.PricingQuotes.AnyAsync(x => !x.IsDeleted && x.CustomerCompanyId == companyId && x.Id != model.Id && x.QuoteNumber == model.QuoteNumber)) return false;

            entity.QuoteNumber = model.QuoteNumber.Trim();
            entity.AgreementId = model.AgreementId;
            entity.OrderId = model.OrderId;
            entity.ShipmentId = model.ShipmentId;
            entity.ServiceLevelId = model.ServiceLevelId;
            entity.Status = model.Status;
            entity.Currency = model.Currency.Trim().ToUpperInvariant();
            entity.ValidUntilUtc = model.ValidUntilUtc;
            entity.NetAmount = model.NetAmount;
            entity.TaxAmount = model.TaxAmount;
            entity.TotalAmount = model.TotalAmount;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePricingQuoteAsync(Guid companyId, Guid pricingQuoteId)
        {
            var entity = await _dbContext.PricingQuotes.Include(x => x.Lines).FirstOrDefaultAsync(x => !x.IsDeleted && x.CustomerCompanyId == companyId && x.Id == pricingQuoteId);
            if (entity == null) return false;
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            foreach (var item in entity.Lines.Where(x => !x.IsDeleted))
            {
                item.IsDeleted = true;
                item.DeletedAtUtc = DateTime.UtcNow;
            }
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<PricingQuoteLineCreateViewModel> GetCreatePricingQuoteLineModelAsync(Guid companyId)
        {
            var model = new PricingQuoteLineCreateViewModel();
            await PopulatePricingQuoteLineOptionsAsync(companyId, model);
            return model;
        }

        public async Task<Guid?> CreatePricingQuoteLineAsync(Guid companyId, PricingQuoteLineCreateViewModel model)
        {
            var quote = await _dbContext.PricingQuotes.FirstOrDefaultAsync(x => !x.IsDeleted && x.CustomerCompanyId == companyId && x.Id == model.PricingQuoteId);
            if (quote == null) return null;

            var entity = new PricingQuoteLine
            {
                PricingQuoteId = model.PricingQuoteId,
                LineNo = model.LineNo,
                LineType = model.LineType,
                Description = model.Description.Trim(),
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                LineAmount = model.LineAmount,
                ReferenceCode = string.IsNullOrWhiteSpace(model.ReferenceCode) ? null : model.ReferenceCode.Trim(),
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };
            await _dbContext.PricingQuoteLines.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<PricingQuoteLineEditViewModel?> GetPricingQuoteLineForEditAsync(Guid companyId, Guid pricingQuoteLineId)
        {
            var model = await _dbContext.PricingQuoteLines.AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == pricingQuoteLineId && x.PricingQuote.CustomerCompanyId == companyId)
                .Select(x => new PricingQuoteLineEditViewModel
                {
                    Id = x.Id,
                    PricingQuoteId = x.PricingQuoteId,
                    LineNo = x.LineNo,
                    LineType = x.LineType,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    LineAmount = x.LineAmount,
                    ReferenceCode = x.ReferenceCode,
                    Notes = x.Notes
                }).FirstOrDefaultAsync();
            if (model == null) return null;
            await PopulatePricingQuoteLineOptionsAsync(companyId, model);
            return model;
        }

        public async Task<bool> UpdatePricingQuoteLineAsync(Guid companyId, PricingQuoteLineEditViewModel model)
        {
            var entity = await _dbContext.PricingQuoteLines.Include(x => x.PricingQuote).FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);
            if (entity == null || entity.PricingQuote.CustomerCompanyId != companyId) return false;
            if (!await _dbContext.PricingQuotes.AnyAsync(x => !x.IsDeleted && x.CustomerCompanyId == companyId && x.Id == model.PricingQuoteId)) return false;

            entity.PricingQuoteId = model.PricingQuoteId;
            entity.LineNo = model.LineNo;
            entity.LineType = model.LineType;
            entity.Description = model.Description.Trim();
            entity.Quantity = model.Quantity;
            entity.UnitPrice = model.UnitPrice;
            entity.LineAmount = model.LineAmount;
            entity.ReferenceCode = string.IsNullOrWhiteSpace(model.ReferenceCode) ? null : model.ReferenceCode.Trim();
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePricingQuoteLineAsync(Guid companyId, Guid pricingQuoteLineId)
        {
            var entity = await _dbContext.PricingQuoteLines.Include(x => x.PricingQuote).FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == pricingQuoteLineId);
            if (entity == null || entity.PricingQuote.CustomerCompanyId != companyId) return false;
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        // Private Helpers

        #region Private Helpers

        private static bool CanEditOrder(OrderStatus status)
            => status == OrderStatus.Draft || status == OrderStatus.Submitted;
        private static bool CanManageOrderLines(OrderStatus status)
            => status == OrderStatus.Draft || status == OrderStatus.Submitted;
        
        private IQueryable<Domain.Shipments.Shipment> GetCompanyAccessibleShipments(Guid companyId)
        {
            return _dbContext.Shipments
                .Where(s => !s.IsDeleted &&
                       (s.CustomerCompanyId == companyId ||
                        s.Parties.Any(p => p.CompanyId == companyId)));
        }
        private async Task<IEnumerable<SelectListItem>> GetShipmentOrderOptionsAsync(Guid companyId)
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"{x.OrderNo} | {x.Status}"
                })
                .ToListAsync();
        }
        private async Task PopulateShipmentFormOptionsAsync(Guid companyId, CompanyShipmentCreateViewModel model)
        {
            model.OrderOptions = await GetShipmentOrderOptionsAsync(companyId);
            model.AddressOptions = await GetAddressOptionsAsync();
        }
        private async Task PopulateShipmentFormOptionsAsync( Guid companyId, CompanyShipmentEditViewModel model)
        {
            model.OrderOptions = await GetShipmentOrderOptionsAsync(companyId);
            model.AddressOptions = await GetAddressOptionsAsync();
        }
        private async Task<bool> CanAccessShipmentAsync(Guid companyId, Guid shipmentId)
        {
            return await GetCompanyAccessibleShipments(companyId)
                .AnyAsync(x => x.Id == shipmentId);
        }
        private async Task<bool> CanManageShipmentAsync(Guid companyId, Guid shipmentId)
        {
            return await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted &&
                               x.Id == shipmentId &&
                               x.CustomerCompanyId == companyId);
        }
        private async Task<IEnumerable<SelectListItem>> GetShipmentPartyCompanyOptionsAsync()
        {
            return await _dbContext.Companies
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<SelectListItem>> GetShipmentPartyContactOptionsAsync(Guid selectedCompanyId)
        {
            return await _dbContext.CompanyContacts
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == selectedCompanyId)
                .OrderBy(x => x.FullName)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = string.IsNullOrWhiteSpace(x.FullName.Trim())
                        ? x.Email
                        : (x.FullName).Trim()
                })
                .ToListAsync();
        }
        private async Task PopulateShipmentPartyCreateOptionsAsync(CompanyShipmentPartyCreateViewModel model)
        {
            model.CompanyOptions = await GetShipmentPartyCompanyOptionsAsync();
            model.ContactOptions = model.CompanyId == Guid.Empty
                ? new List<SelectListItem>()
                : await GetShipmentPartyContactOptionsAsync(model.CompanyId);
        }
        private async Task PopulateShipmentPartyEditOptionsAsync(CompanyShipmentPartyEditViewModel model)
        {
            model.CompanyOptions = await GetShipmentPartyCompanyOptionsAsync();
            model.ContactOptions = model.CompanyId == Guid.Empty
                ? new List<SelectListItem>()
                : await GetShipmentPartyContactOptionsAsync(model.CompanyId);
        }
        private async Task<IEnumerable<SelectListItem>> GetLocationOptionsAsync()
        {
            return await _dbContext.Locations
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();
        }
        private async Task PopulateShipmentLegCreateOptionsAsync(CompanyShipmentLegCreateViewModel model)
        {
            model.LocationOptions = await GetLocationOptionsAsync();
        }
        private async Task PopulateShipmentLegEditOptionsAsync(CompanyShipmentLegEditViewModel model)
        {
            model.LocationOptions = await GetLocationOptionsAsync();
        }
        private async Task<int> GetNextShipmentLegNoAsync(Guid shipmentId)
        {
            var maxLegNo = await _dbContext.ShipmentLegs
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .Select(x => (int?)x.LegNo)
                .MaxAsync();

            return (maxLegNo ?? 0) + 1;
        }
        private static bool CanChangeShipmentStatus(ShipmentStatus status)
            => status != ShipmentStatus.Delivered && status != ShipmentStatus.Cancelled;
        private static bool CanTransitionShipmentStatus(ShipmentStatus current, ShipmentStatus next)
        {
            if (current == next) return false;
            if (current == ShipmentStatus.Delivered || current == ShipmentStatus.Cancelled) return false;

            return true;
        }
        private async Task PopulateTrackingEventCreateOptionsAsync(CompanyTrackingEventCreateViewModel model)
        {
            model.LocationOptions = await GetLocationOptionsAsync();
        }
        private async Task PopulateTrackingEventEditOptionsAsync(CompanyTrackingEventEditViewModel model)
        {
            model.LocationOptions = await GetLocationOptionsAsync();
        }
        private async Task<(IList<CompanyShipmentTripOptionViewModel> Trips, IList<CompanyShipmentLegOptionViewModel> Legs)> GetShipmentTripFormOptionsAsync(Guid shipmentId)
        {
            var tripOptions = await _dbContext.Trips
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new CompanyShipmentTripOptionViewModel
                {
                    Id = x.Id,
                    DisplayName = x.Id.ToString()
                })
                .ToListAsync();

            var legOptions = await _dbContext.ShipmentLegs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderBy(x => x.LegNo)
                .Select(x => new CompanyShipmentLegOptionViewModel
                {
                    Id = x.Id,
                    DisplayName = $"Leg {x.LegNo} - {x.Mode}"
                })
                .ToListAsync();

            return (tripOptions, legOptions);
        }
        private async Task<List<SelectListItem>> BuildShipmentLegOptionsAsync(Guid shipmentId, Guid? selectedLegId = null)
        {
            var items = await _dbContext.ShipmentLegs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderBy(x => x.LegNo)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = $"Leg #{x.LegNo} | {x.Mode} | {x.Id}",
                    Selected = selectedLegId.HasValue && x.Id == selectedLegId.Value
                })
                .ToListAsync();

            items.Insert(0, new SelectListItem
            {
                Value = "",
                Text = "-- No shipment leg --",
                Selected = !selectedLegId.HasValue
            });

            return items;
        }
        private async Task<(IList<CompanyContainerOptionViewModel> Containers, IList<CompanyShipmentLegOptionViewModel> Legs)> GetShipmentContainerFormOptionsAsync(Guid shipmentId)
        {
            var containerOptions = await _dbContext.Containers
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new CompanyContainerOptionViewModel
                {
                    Id = x.Id,
                    DisplayName = x.Id.ToString()
                })
                .ToListAsync();

            var legOptions = await _dbContext.ShipmentLegs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderBy(x => x.LegNo)
                .Select(x => new CompanyShipmentLegOptionViewModel
                {
                    Id = x.Id,
                    DisplayName = $"Leg {x.LegNo} - {x.Mode}"
                })
                .ToListAsync();

            return (containerOptions, legOptions);
        }
        private async Task<(IList<CompanyUldOptionViewModel> Ulds, IList<CompanyShipmentLegOptionViewModel> Legs)> GetShipmentUldFormOptionsAsync(Guid shipmentId)
        {
            var uldOptions = await _dbContext.ULDs
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new CompanyUldOptionViewModel
                {
                    Id = x.Id,
                    DisplayName = x.Id.ToString()
                })
                .ToListAsync();

            var legOptions = await _dbContext.ShipmentLegs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderBy(x => x.LegNo)
                .Select(x => new CompanyShipmentLegOptionViewModel
                {
                    Id = x.Id,
                    DisplayName = $"Leg {x.LegNo} - {x.Mode}"
                })
                .ToListAsync();

            return (uldOptions, legOptions);
        }
        #endregion

        // PACKAGE ITEMS

        #region Package Items

        public async Task<CompanyPackageItemsViewModel?> GetPackageItemsAsync(Guid companyId, Guid packageId)
        {
            var package = await _dbContext.Packages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == packageId)
                .Select(x => new
                {
                    x.Id,
                    x.PackageNo,
                    x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    CustomerCompanyId = x.Shipment.CustomerCompanyId
                })
                .FirstOrDefaultAsync();

            if (package == null)
            {
                return null;
            }

            var hasAccess = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .AnyAsync(x => x.Id == package.ShipmentId);

            if (!hasAccess)
            {
                return null;
            }

            var items = await _dbContext.PackageItems
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.PackageId == packageId)
                .OrderBy(x => x.Description)
                .Select(x => new CompanyPackageItemListItemViewModel
                {
                    Id = x.Id,
                    PackageId = x.PackageId,
                    ShipmentId = x.Package.ShipmentId,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    Unit = x.Unit,
                    HsCode = x.HsCode,
                    OriginCountry = x.OriginCountry,
                    UnitPrice = x.UnitPrice,
                    Currency = x.Currency
                })
                .ToListAsync();

            return new CompanyPackageItemsViewModel
            {
                ShipmentId = package.ShipmentId,
                ShipmentNo = package.ShipmentNo,
                PackageId = package.Id,
                PackageNo = package.PackageNo,
                CanManagePackageItems = package.CustomerCompanyId == companyId,
                PackageItems = items
            };
        }

        public async Task<CompanyPackageItemCreateViewModel?> GetCreatePackageItemModelAsync(Guid companyId, Guid packageId)
        {
            var package = await _dbContext.Packages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == packageId)
                .Select(x => new
                {
                    x.Id,
                    x.PackageNo,
                    x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (package == null)
            {
                return null;
            }

            if (!await CanManageShipmentAsync(companyId, package.ShipmentId))
            {
                return null;
            }

            return new CompanyPackageItemCreateViewModel
            {
                PackageId = package.Id,
                PackageNo = package.PackageNo,
                ShipmentId = package.ShipmentId,
                ShipmentNo = package.ShipmentNo,
                Quantity = 1
            };
        }

        public async Task<Guid?> CreatePackageItemAsync(Guid companyId, CompanyPackageItemCreateViewModel model)
        {
            var package = await _dbContext.Packages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == model.PackageId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentId
                })
                .FirstOrDefaultAsync();

            if (package == null)
            {
                return null;
            }

            if (!await CanManageShipmentAsync(companyId, package.ShipmentId))
            {
                return null;
            }

            var entity = new PackageItem
            {
                PackageId = model.PackageId,
                Description = model.Description.Trim(),
                Quantity = model.Quantity,
                Unit = string.IsNullOrWhiteSpace(model.Unit) ? null : model.Unit.Trim(),
                HsCode = string.IsNullOrWhiteSpace(model.HsCode) ? null : model.HsCode.Trim(),
                OriginCountry = string.IsNullOrWhiteSpace(model.OriginCountry) ? null : model.OriginCountry.Trim(),
                UnitPrice = model.UnitPrice,
                Currency = string.IsNullOrWhiteSpace(model.Currency) ? null : model.Currency.Trim().ToUpper()
            };

            await _dbContext.PackageItems.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<CompanyPackageItemEditViewModel?> GetPackageItemForEditAsync(Guid companyId, Guid packageItemId)
        {
            var model = await _dbContext.PackageItems
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == packageItemId)
                .Select(x => new CompanyPackageItemEditViewModel
                {
                    Id = x.Id,
                    PackageId = x.PackageId,
                    ShipmentId = x.Package.ShipmentId,
                    ShipmentNo = x.Package.Shipment.ShipmentNo,
                    PackageNo = x.Package.PackageNo,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    Unit = x.Unit,
                    HsCode = x.HsCode,
                    OriginCountry = x.OriginCountry,
                    UnitPrice = x.UnitPrice,
                    Currency = x.Currency
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            return model;
        }

        public async Task<bool> UpdatePackageItemAsync(Guid companyId, CompanyPackageItemEditViewModel model)
        {
            var entity = await _dbContext.PackageItems
                .Include(x => x.Package)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.Package.ShipmentId))
            {
                return false;
            }

            entity.Description = model.Description.Trim();
            entity.Quantity = model.Quantity;
            entity.Unit = string.IsNullOrWhiteSpace(model.Unit) ? null : model.Unit.Trim();
            entity.HsCode = string.IsNullOrWhiteSpace(model.HsCode) ? null : model.HsCode.Trim();
            entity.OriginCountry = string.IsNullOrWhiteSpace(model.OriginCountry) ? null : model.OriginCountry.Trim();
            entity.UnitPrice = model.UnitPrice;
            entity.Currency = string.IsNullOrWhiteSpace(model.Currency) ? null : model.Currency.Trim().ToUpper();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePackageItemAsync(Guid companyId, Guid packageItemId)
        {
            var entity = await _dbContext.PackageItems
                .Include(x => x.Package)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == packageItemId);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.Package.ShipmentId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        // CARGO ITEMS

        #region Cargo Items

        public async Task<CompanyCargoItemsViewModel?> GetCargoItemsAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    CanManage = x.CustomerCompanyId == companyId
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var cargoItems = await _dbContext.CargoItems
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderBy(x => x.Description)
                .Select(x => new CompanyCargoItemListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    Description = x.Description,
                    CargoItemType = x.CargoItemType.ToString(),
                    Quantity = x.Quantity,
                    UnitOfMeasure = x.UnitOfMeasure,
                    GrossWeightKg = x.GrossWeightKg,
                    NetWeightKg = x.NetWeightKg,
                    VolumeCbm = x.VolumeCbm,
                    LengthCm = x.LengthCm,
                    WidthCm = x.WidthCm,
                    HeightCm = x.HeightCm,
                    HsCode = x.HsCode,
                    OriginCountry = x.OriginCountry,
                    IsStackable = x.IsStackable,
                    IsFragile = x.IsFragile,
                    Notes = x.Notes
                })
                .ToListAsync();

            return new CompanyCargoItemsViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CanManageCargoItems = shipment.CanManage,
                CargoItems = cargoItems
            };
        }

        public async Task<CompanyCargoItemCreateViewModel?> GetCreateCargoItemModelAsync(Guid companyId, Guid shipmentId)
        {
            if (!await CanManageShipmentAsync(companyId, shipmentId))
            {
                return null;
            }

            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            return new CompanyCargoItemCreateViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                IsStackable = true,
                IsFragile = false
            };
        }

        public async Task<Guid?> CreateCargoItemAsync(Guid companyId, CompanyCargoItemCreateViewModel model)
        {
            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId);

            if (!shipmentExists)
            {
                return null;
            }

            var entity = new CargoItem
            {
                ShipmentId = model.ShipmentId,
                Description = model.Description.Trim(),
                CargoItemType = model.CargoItemType,
                Quantity = model.Quantity,
                UnitOfMeasure = string.IsNullOrWhiteSpace(model.UnitOfMeasure) ? null : model.UnitOfMeasure.Trim(),
                GrossWeightKg = model.GrossWeightKg,
                NetWeightKg = model.NetWeightKg,
                VolumeCbm = model.VolumeCbm,
                LengthCm = model.LengthCm,
                WidthCm = model.WidthCm,
                HeightCm = model.HeightCm,
                HsCode = string.IsNullOrWhiteSpace(model.HsCode) ? null : model.HsCode.Trim(),
                OriginCountry = string.IsNullOrWhiteSpace(model.OriginCountry) ? null : model.OriginCountry.Trim(),
                IsStackable = model.IsStackable,
                IsFragile = model.IsFragile,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            await _dbContext.CargoItems.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<CompanyCargoItemEditViewModel?> GetCargoItemForEditAsync(Guid companyId, Guid cargoItemId)
        {
            var model = await _dbContext.CargoItems
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == cargoItemId)
                .Select(x => new CompanyCargoItemEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    Description = x.Description,
                    CargoItemType = x.CargoItemType,
                    Quantity = x.Quantity,
                    UnitOfMeasure = x.UnitOfMeasure,
                    GrossWeightKg = x.GrossWeightKg,
                    NetWeightKg = x.NetWeightKg,
                    VolumeCbm = x.VolumeCbm,
                    LengthCm = x.LengthCm,
                    WidthCm = x.WidthCm,
                    HeightCm = x.HeightCm,
                    HsCode = x.HsCode,
                    OriginCountry = x.OriginCountry,
                    IsStackable = x.IsStackable,
                    IsFragile = x.IsFragile,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            return model;
        }

        public async Task<bool> UpdateCargoItemAsync(Guid companyId, CompanyCargoItemEditViewModel model)
        {
            var entity = await _dbContext.CargoItems
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            entity.Description = model.Description.Trim();
            entity.CargoItemType = model.CargoItemType;
            entity.Quantity = model.Quantity;
            entity.UnitOfMeasure = string.IsNullOrWhiteSpace(model.UnitOfMeasure) ? null : model.UnitOfMeasure.Trim();
            entity.GrossWeightKg = model.GrossWeightKg;
            entity.NetWeightKg = model.NetWeightKg;
            entity.VolumeCbm = model.VolumeCbm;
            entity.LengthCm = model.LengthCm;
            entity.WidthCm = model.WidthCm;
            entity.HeightCm = model.HeightCm;
            entity.HsCode = string.IsNullOrWhiteSpace(model.HsCode) ? null : model.HsCode.Trim();
            entity.OriginCountry = string.IsNullOrWhiteSpace(model.OriginCountry) ? null : model.OriginCountry.Trim();
            entity.IsStackable = model.IsStackable;
            entity.IsFragile = model.IsFragile;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCargoItemAsync(Guid companyId, Guid cargoItemId)
        {
            var entity = await _dbContext.CargoItems
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == cargoItemId);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        #endregion

        // SHIPMENT REFERENCES

        #region Shipment References

        public async Task<CompanyShipmentReferencesViewModel?> GetShipmentReferencesAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    CanManage = x.CustomerCompanyId == companyId
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var references = await _dbContext.ShipmentReferences
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderBy(x => x.ReferenceType)
                .ThenBy(x => x.ReferenceValue)
                .Select(x => new CompanyShipmentReferenceListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ReferenceType = x.ReferenceType.ToString(),
                    ReferenceValue = x.ReferenceValue,
                    Description = x.Description
                })
                .ToListAsync();

            return new CompanyShipmentReferencesViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CanManageReferences = shipment.CanManage,
                References = references
            };
        }

        public async Task<CompanyShipmentReferenceCreateViewModel?> GetCreateShipmentReferenceModelAsync(Guid companyId, Guid shipmentId)
        {
            if (!await CanManageShipmentAsync(companyId, shipmentId))
            {
                return null;
            }

            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            return new CompanyShipmentReferenceCreateViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo
            };
        }

        public async Task<Guid?> CreateShipmentReferenceAsync(Guid companyId, CompanyShipmentReferenceCreateViewModel model)
        {
            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId);

            if (!shipmentExists)
            {
                return null;
            }

            var entity = new ShipmentReference
            {
                ShipmentId = model.ShipmentId,
                ReferenceType = model.ReferenceType,
                ReferenceValue = model.ReferenceValue.Trim(),
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim()
            };

            await _dbContext.ShipmentReferences.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<CompanyShipmentReferenceEditViewModel?> GetShipmentReferenceForEditAsync(Guid companyId, Guid shipmentReferenceId)
        {
            var model = await _dbContext.ShipmentReferences
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentReferenceId)
                .Select(x => new CompanyShipmentReferenceEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    ReferenceType = x.ReferenceType,
                    ReferenceValue = x.ReferenceValue,
                    Description = x.Description
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            return model;
        }

        public async Task<bool> UpdateShipmentReferenceAsync(Guid companyId, CompanyShipmentReferenceEditViewModel model)
        {
            var entity = await _dbContext.ShipmentReferences
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            entity.ReferenceType = model.ReferenceType;
            entity.ReferenceValue = model.ReferenceValue.Trim();
            entity.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteShipmentReferenceAsync(Guid companyId, Guid shipmentReferenceId)
        {
            var entity = await _dbContext.ShipmentReferences
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == shipmentReferenceId);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        // SHIPMENT TAGS

        #region Shipment Tags

        public async Task<CompanyShipmentTagsViewModel?> GetShipmentTagsAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    CanManage = x.CustomerCompanyId == companyId
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var tags = await _dbContext.ShipmentTags
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderBy(x => x.TagType)
                .ThenBy(x => x.CustomValue)
                .Select(x => new CompanyShipmentTagListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    TagType = x.TagType.ToString(),
                    CustomValue = x.CustomValue,
                    Notes = x.Notes
                })
                .ToListAsync();

            return new CompanyShipmentTagsViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CanManageTags = shipment.CanManage,
                Tags = tags
            };
        }

        public async Task<CompanyShipmentTagCreateViewModel?> GetCreateShipmentTagModelAsync(Guid companyId, Guid shipmentId)
        {
            if (!await CanManageShipmentAsync(companyId, shipmentId))
            {
                return null;
            }

            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            return new CompanyShipmentTagCreateViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo
            };
        }

        public async Task<Guid?> CreateShipmentTagAsync(Guid companyId, CompanyShipmentTagCreateViewModel model)
        {
            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId);

            if (!shipmentExists)
            {
                return null;
            }

            var entity = new ShipmentTag
            {
                ShipmentId = model.ShipmentId,
                TagType = model.TagType,
                CustomValue = string.IsNullOrWhiteSpace(model.CustomValue) ? null : model.CustomValue.Trim(),
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            await _dbContext.ShipmentTags.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<CompanyShipmentTagEditViewModel?> GetShipmentTagForEditAsync(Guid companyId, Guid shipmentTagId)
        {
            var model = await _dbContext.ShipmentTags
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentTagId)
                .Select(x => new CompanyShipmentTagEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    TagType = x.TagType,
                    CustomValue = x.CustomValue,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            return model;
        }

        public async Task<bool> UpdateShipmentTagAsync(Guid companyId, CompanyShipmentTagEditViewModel model)
        {
            var entity = await _dbContext.ShipmentTags
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            entity.TagType = model.TagType;
            entity.CustomValue = string.IsNullOrWhiteSpace(model.CustomValue) ? null : model.CustomValue.Trim();
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteShipmentTagAsync(Guid companyId, Guid shipmentTagId)
        {
            var entity = await _dbContext.ShipmentTags
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == shipmentTagId);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        #endregion

        // SHIPMENT VOYAGES

        #region Shipment Voyages

        public async Task<CompanyShipmentVoyagesViewModel?> GetShipmentVoyagesAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    CanManage = x.CustomerCompanyId == companyId
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var voyages = await _dbContext.ShipmentVoyages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderBy(x => x.BookingReference)
                .ThenBy(x => x.CreatedAtUtc)
                .Select(x => new CompanyShipmentVoyageListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    VoyageId = x.VoyageId,
                    ShipmentLegId = x.ShipmentLegId,
                    BookingReference = x.BookingReference,
                    Notes = x.Notes
                })
                .ToListAsync();

            return new CompanyShipmentVoyagesViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CanManageVoyages = shipment.CanManage,
                Voyages = voyages
            };
        }

        public async Task<CompanyShipmentVoyageCreateViewModel?> GetCreateShipmentVoyageModelAsync(Guid companyId, Guid shipmentId)
        {
            if (!await CanManageShipmentAsync(companyId, shipmentId))
            {
                return null;
            }

            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var usedVoyageIds = await _dbContext.ShipmentVoyages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .Select(x => x.VoyageId)
                .Distinct()
                .ToListAsync();

            var nextAvailableVoyageId = await _dbContext.Voyages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && !usedVoyageIds.Contains(x.Id))
                .OrderBy(x => x.CreatedAtUtc)
                .Select(x => (Guid?)x.Id)
                .FirstOrDefaultAsync();

            if (!nextAvailableVoyageId.HasValue)
            {
                return null;
            }

            return new CompanyShipmentVoyageCreateViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                VoyageId = nextAvailableVoyageId.Value,
                ShipmentLegOptions = await BuildShipmentLegOptionsAsync(shipment.Id)
            };
        }

        public async Task<Guid?> CreateShipmentVoyageAsync(Guid companyId, CompanyShipmentVoyageCreateViewModel model)
        {
            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId);

            if (!shipmentExists)
            {
                return null;
            }

            var voyageExists = await _dbContext.Voyages
                .AnyAsync(x => !x.IsDeleted && x.Id == model.VoyageId);

            if (!voyageExists)
            {
                return null;
            }

            if (model.ShipmentLegId.HasValue)
            {
                var legExists = await _dbContext.ShipmentLegs
                    .AnyAsync(x => !x.IsDeleted
                                   && x.Id == model.ShipmentLegId.Value
                                   && x.ShipmentId == model.ShipmentId);

                if (!legExists)
                {
                    return null;
                }
            }

            var entity = new ShipmentVoyage
            {
                ShipmentId = model.ShipmentId,
                VoyageId = model.VoyageId,
                ShipmentLegId = model.ShipmentLegId,
                BookingReference = string.IsNullOrWhiteSpace(model.BookingReference) ? null : model.BookingReference.Trim(),
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            await _dbContext.ShipmentVoyages.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<CompanyShipmentVoyageEditViewModel?> GetShipmentVoyageForEditAsync(Guid companyId, Guid shipmentVoyageId)
        {
            var model = await _dbContext.ShipmentVoyages
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentVoyageId)
                .Select(x => new CompanyShipmentVoyageEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    VoyageId = x.VoyageId,
                    ShipmentLegId = x.ShipmentLegId,
                    BookingReference = x.BookingReference,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            model.ShipmentLegOptions = await BuildShipmentLegOptionsAsync(model.ShipmentId, model.ShipmentLegId);

            return model;
        }

        public async Task<bool> UpdateShipmentVoyageAsync(Guid companyId, CompanyShipmentVoyageEditViewModel model)
        {
            var entity = await _dbContext.ShipmentVoyages
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            var voyageExists = await _dbContext.Voyages
                .AnyAsync(x => !x.IsDeleted && x.Id == model.VoyageId);

            if (!voyageExists)
            {
                return false;
            }

            if (model.ShipmentLegId.HasValue)
            {
                var legExists = await _dbContext.ShipmentLegs
                    .AnyAsync(x => !x.IsDeleted
                                   && x.Id == model.ShipmentLegId.Value
                                   && x.ShipmentId == entity.ShipmentId);

                if (!legExists)
                {
                    return false;
                }
            }

            entity.VoyageId = model.VoyageId;
            entity.ShipmentLegId = model.ShipmentLegId;
            entity.BookingReference = string.IsNullOrWhiteSpace(model.BookingReference) ? null : model.BookingReference.Trim();
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteShipmentVoyageAsync(Guid companyId, Guid shipmentVoyageId)
        {
            var entity = await _dbContext.ShipmentVoyages
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == shipmentVoyageId);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        #endregion

        // SHIPMENT TRIPS

        #region Shipment Trips

        public async Task<CompanyShipmentTripsViewModel?> GetShipmentTripsAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    CanManage = x.CustomerCompanyId == companyId
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var trips = await _dbContext.ShipmentTrips
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new CompanyShipmentTripListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    TripId = x.TripId,
                    ShipmentLegId = x.ShipmentLegId,
                    TripDisplay = x.TripId.ToString(),
                    ShipmentLegDisplay = x.ShipmentLegId.HasValue
                        ? $"Leg {x.ShipmentLeg!.LegNo}"
                        : null,
                    Notes = x.Notes
                })
                .ToListAsync();

            return new CompanyShipmentTripsViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CanManageTrips = shipment.CanManage,
                Trips = trips
            };
        }

        public async Task<CompanyShipmentTripCreateViewModel?> GetCreateShipmentTripModelAsync(Guid companyId, Guid shipmentId)
        {
            if (!await CanManageShipmentAsync(companyId, shipmentId))
            {
                return null;
            }

            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var options = await GetShipmentTripFormOptionsAsync(shipmentId);

            return new CompanyShipmentTripCreateViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                TripOptions = options.Trips,
                ShipmentLegOptions = options.Legs
            };
        }

        public async Task<Guid?> CreateShipmentTripAsync(Guid companyId, CompanyShipmentTripCreateViewModel model)
        {
            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId);

            if (!shipmentExists)
            {
                return null;
            }

            var tripExists = await _dbContext.Trips
                .AnyAsync(x => !x.IsDeleted && x.Id == model.TripId);

            if (!tripExists)
            {
                return null;
            }

            if (model.ShipmentLegId.HasValue)
            {
                var legExists = await _dbContext.ShipmentLegs
                    .AnyAsync(x => !x.IsDeleted
                                   && x.Id == model.ShipmentLegId.Value
                                   && x.ShipmentId == model.ShipmentId);

                if (!legExists)
                {
                    return null;
                }
            }

            var entity = new ShipmentTrip
            {
                ShipmentId = model.ShipmentId,
                TripId = model.TripId,
                ShipmentLegId = model.ShipmentLegId,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            await _dbContext.ShipmentTrips.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<CompanyShipmentTripEditViewModel?> GetShipmentTripForEditAsync(Guid companyId, Guid shipmentTripId)
        {
            var model = await _dbContext.ShipmentTrips
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentTripId)
                .Select(x => new CompanyShipmentTripEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    TripId = x.TripId,
                    ShipmentLegId = x.ShipmentLegId,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            var options = await GetShipmentTripFormOptionsAsync(model.ShipmentId);
            model.TripOptions = options.Trips;
            model.ShipmentLegOptions = options.Legs;

            return model;
        }

        public async Task<bool> UpdateShipmentTripAsync(Guid companyId, CompanyShipmentTripEditViewModel model)
        {
            var entity = await _dbContext.ShipmentTrips
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            var tripExists = await _dbContext.Trips
                .AnyAsync(x => !x.IsDeleted && x.Id == model.TripId);

            if (!tripExists)
            {
                return false;
            }

            if (model.ShipmentLegId.HasValue)
            {
                var legExists = await _dbContext.ShipmentLegs
                    .AnyAsync(x => !x.IsDeleted
                                   && x.Id == model.ShipmentLegId.Value
                                   && x.ShipmentId == entity.ShipmentId);

                if (!legExists)
                {
                    return false;
                }
            }

            entity.TripId = model.TripId;
            entity.ShipmentLegId = model.ShipmentLegId;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteShipmentTripAsync(Guid companyId, Guid shipmentTripId)
        {
            var entity = await _dbContext.ShipmentTrips
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == shipmentTripId);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        #endregion

        // SHIPMENT CONTAINERS

        #region Shipment Containers

        public async Task<CompanyShipmentContainersViewModel?> GetShipmentContainersAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    CanManage = x.CustomerCompanyId == companyId
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var containers = await _dbContext.ShipmentContainers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new CompanyShipmentContainerListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ContainerId = x.ContainerId,
                    ShipmentLegId = x.ShipmentLegId,
                    ContainerDisplay = x.ContainerId.ToString(),
                    ShipmentLegDisplay = x.ShipmentLegId.HasValue
                        ? $"Leg {x.ShipmentLeg!.LegNo}"
                        : null,
                    GrossWeightKg = x.GrossWeightKg,
                    SealNumber = x.SealNumber,
                    Notes = x.Notes
                })
                .ToListAsync();

            return new CompanyShipmentContainersViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CanManageContainers = shipment.CanManage,
                Containers = containers
            };
        }

        public async Task<CompanyShipmentContainerCreateViewModel?> GetCreateShipmentContainerModelAsync(Guid companyId, Guid shipmentId)
        {
            if (!await CanManageShipmentAsync(companyId, shipmentId))
            {
                return null;
            }

            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var options = await GetShipmentContainerFormOptionsAsync(shipmentId);

            return new CompanyShipmentContainerCreateViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                ContainerOptions = options.Containers,
                ShipmentLegOptions = options.Legs
            };
        }

        public async Task<Guid?> CreateShipmentContainerAsync(Guid companyId, CompanyShipmentContainerCreateViewModel model)
        {
            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId);

            if (!shipmentExists)
            {
                return null;
            }

            var containerExists = await _dbContext.Containers
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ContainerId);

            if (!containerExists)
            {
                return null;
            }

            if (model.ShipmentLegId.HasValue)
            {
                var legExists = await _dbContext.ShipmentLegs
                    .AnyAsync(x => !x.IsDeleted
                                   && x.Id == model.ShipmentLegId.Value
                                   && x.ShipmentId == model.ShipmentId);

                if (!legExists)
                {
                    return null;
                }
            }

            var entity = new ShipmentContainer
            {
                ShipmentId = model.ShipmentId,
                ContainerId = model.ContainerId,
                ShipmentLegId = model.ShipmentLegId,
                GrossWeightKg = model.GrossWeightKg,
                SealNumber = string.IsNullOrWhiteSpace(model.SealNumber) ? null : model.SealNumber.Trim(),
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            await _dbContext.ShipmentContainers.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<CompanyShipmentContainerEditViewModel?> GetShipmentContainerForEditAsync(Guid companyId, Guid shipmentContainerId)
        {
            var model = await _dbContext.ShipmentContainers
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentContainerId)
                .Select(x => new CompanyShipmentContainerEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    ContainerId = x.ContainerId,
                    ShipmentLegId = x.ShipmentLegId,
                    GrossWeightKg = x.GrossWeightKg,
                    SealNumber = x.SealNumber,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            var options = await GetShipmentContainerFormOptionsAsync(model.ShipmentId);
            model.ContainerOptions = options.Containers;
            model.ShipmentLegOptions = options.Legs;

            return model;
        }

        public async Task<bool> UpdateShipmentContainerAsync(Guid companyId, CompanyShipmentContainerEditViewModel model)
        {
            var entity = await _dbContext.ShipmentContainers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            var containerExists = await _dbContext.Containers
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ContainerId);

            if (!containerExists)
            {
                return false;
            }

            if (model.ShipmentLegId.HasValue)
            {
                var legExists = await _dbContext.ShipmentLegs
                    .AnyAsync(x => !x.IsDeleted
                                   && x.Id == model.ShipmentLegId.Value
                                   && x.ShipmentId == entity.ShipmentId);

                if (!legExists)
                {
                    return false;
                }
            }

            entity.ContainerId = model.ContainerId;
            entity.ShipmentLegId = model.ShipmentLegId;
            entity.GrossWeightKg = model.GrossWeightKg;
            entity.SealNumber = string.IsNullOrWhiteSpace(model.SealNumber) ? null : model.SealNumber.Trim();
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteShipmentContainerAsync(Guid companyId, Guid shipmentContainerId)
        {
            var entity = await _dbContext.ShipmentContainers
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == shipmentContainerId);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        #endregion

        // SHIPMENT ULDS

        #region Shipment ULDs

        public async Task<CompanyShipmentUldsViewModel?> GetShipmentUldsAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo,
                    CanManage = x.CustomerCompanyId == companyId
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var ulds = await _dbContext.ShipmentULDs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.ShipmentId == shipmentId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new CompanyShipmentUldListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    UldId = x.UldId,
                    ShipmentLegId = x.ShipmentLegId,
                    UldDisplay = x.UldId.ToString(),
                    ShipmentLegDisplay = x.ShipmentLegId.HasValue
                        ? $"Leg {x.ShipmentLeg!.LegNo}"
                        : null,
                    GrossWeightKg = x.GrossWeightKg,
                    VolumeCbm = x.VolumeCbm,
                    Notes = x.Notes
                })
                .ToListAsync();

            return new CompanyShipmentUldsViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CanManageUlds = shipment.CanManage,
                Ulds = ulds
            };
        }

        public async Task<CompanyShipmentUldCreateViewModel?> GetCreateShipmentUldModelAsync(Guid companyId, Guid shipmentId)
        {
            if (!await CanManageShipmentAsync(companyId, shipmentId))
            {
                return null;
            }

            var shipment = await _dbContext.Shipments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var options = await GetShipmentUldFormOptionsAsync(shipmentId);

            return new CompanyShipmentUldCreateViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                UldOptions = options.Ulds,
                ShipmentLegOptions = options.Legs
            };
        }

        public async Task<Guid?> CreateShipmentUldAsync(Guid companyId, CompanyShipmentUldCreateViewModel model)
        {
            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            var shipmentExists = await _dbContext.Shipments
                .AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId);

            if (!shipmentExists)
            {
                return null;
            }

            var uldExists = await _dbContext.ULDs
                .AnyAsync(x => !x.IsDeleted && x.Id == model.UldId);

            if (!uldExists)
            {
                return null;
            }

            if (model.ShipmentLegId.HasValue)
            {
                var legExists = await _dbContext.ShipmentLegs
                    .AnyAsync(x => !x.IsDeleted
                                   && x.Id == model.ShipmentLegId.Value
                                   && x.ShipmentId == model.ShipmentId);

                if (!legExists)
                {
                    return null;
                }
            }

            var entity = new ShipmentULD
            {
                ShipmentId = model.ShipmentId,
                UldId = model.UldId,
                ShipmentLegId = model.ShipmentLegId,
                GrossWeightKg = model.GrossWeightKg,
                VolumeCbm = model.VolumeCbm,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            await _dbContext.ShipmentULDs.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<CompanyShipmentUldEditViewModel?> GetShipmentUldForEditAsync(Guid companyId, Guid shipmentUldId)
        {
            var model = await _dbContext.ShipmentULDs
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == shipmentUldId)
                .Select(x => new CompanyShipmentUldEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    UldId = x.UldId,
                    ShipmentLegId = x.ShipmentLegId,
                    GrossWeightKg = x.GrossWeightKg,
                    VolumeCbm = x.VolumeCbm,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return null;
            }

            if (!await CanManageShipmentAsync(companyId, model.ShipmentId))
            {
                return null;
            }

            var options = await GetShipmentUldFormOptionsAsync(model.ShipmentId);
            model.UldOptions = options.Ulds;
            model.ShipmentLegOptions = options.Legs;

            return model;
        }

        public async Task<bool> UpdateShipmentUldAsync(Guid companyId, CompanyShipmentUldEditViewModel model)
        {
            var entity = await _dbContext.ShipmentULDs
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            var uldExists = await _dbContext.ULDs
                .AnyAsync(x => !x.IsDeleted && x.Id == model.UldId);

            if (!uldExists)
            {
                return false;
            }

            if (model.ShipmentLegId.HasValue)
            {
                var legExists = await _dbContext.ShipmentLegs
                    .AnyAsync(x => !x.IsDeleted
                                   && x.Id == model.ShipmentLegId.Value
                                   && x.ShipmentId == entity.ShipmentId);

                if (!legExists)
                {
                    return false;
                }
            }

            entity.UldId = model.UldId;
            entity.ShipmentLegId = model.ShipmentLegId;
            entity.GrossWeightKg = model.GrossWeightKg;
            entity.VolumeCbm = model.VolumeCbm;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteShipmentUldAsync(Guid companyId, Guid shipmentUldId)
        {
            var entity = await _dbContext.ShipmentULDs
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == shipmentUldId);

            if (entity == null)
            {
                return false;
            }

            if (!await CanManageShipmentAsync(companyId, entity.ShipmentId))
            {
                return false;
            }

            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }
        #endregion  


        // Pricing

        #region Pricing

        public async Task<IEnumerable<ServiceLevelListItemViewModel>> GetServiceLevelsAsync()
        {
            return await _dbContext.ServiceLevels
                .AsNoTracking()
                .OrderBy(x => x.Code)
                .Select(x => new ServiceLevelListItemViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    ServiceLevelType = x.ServiceLevelType.ToString(),
                    TransportMode = x.TransportMode.ToString(),
                    MaxWeightKg = x.MaxWeightKg,
                    EstimatedTransitDays = x.EstimatedTransitDays,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<GeoZoneListItemViewModel>> GetGeoZonesAsync()
        {
            return await _dbContext.GeoZones
                .AsNoTracking()
                .OrderBy(x => x.Code)
                .Select(x => new GeoZoneListItemViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ZoneRuleListItemViewModel>> GetZoneRulesAsync()
        {
            return await _dbContext.ZoneRules
                .AsNoTracking()
                .OrderBy(x => x.GeoZone.Code)
                .ThenBy(x => x.Priority)
                .Select(x => new ZoneRuleListItemViewModel
                {
                    Id = x.Id,
                    GeoZoneCode = x.GeoZone.Code,
                    GeoZoneName = x.GeoZone.Name,
                    Country = x.Country,
                    City = x.City,
                    PostalCodeFrom = x.PostalCodeFrom,
                    PostalCodeTo = x.PostalCodeTo,
                    Priority = x.Priority,
                    ValidFromUtc = x.ValidFromUtc,
                    ValidToUtc = x.ValidToUtc
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TariffListItemViewModel>> GetTariffsAsync()
        {
            return await _dbContext.Tariffs
                .AsNoTracking()
                .OrderBy(x => x.ServiceLevel.Code)
                .ThenBy(x => x.GeoZone.Code)
                .ThenByDescending(x => x.ValidFromUtc)
                .Select(x => new TariffListItemViewModel
                {
                    Id = x.Id,
                    ServiceLevelCode = x.ServiceLevel.Code,
                    GeoZoneCode = x.GeoZone.Code,
                    CalcBasis = x.CalcBasis.ToString(),
                    Currency = x.Currency,
                    ValidFromUtc = x.ValidFromUtc,
                    ValidToUtc = x.ValidToUtc,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TariffRateListItemViewModel>> GetTariffRatesAsync()
        {
            return await _dbContext.TariffRates
                .AsNoTracking()
                .OrderBy(x => x.Tariff.ServiceLevel.Code)
                .ThenBy(x => x.Tariff.GeoZone.Code)
                .ThenBy(x => x.SortOrder)
                .Select(x => new TariffRateListItemViewModel
                {
                    Id = x.Id,
                    TariffId = x.TariffId,
                    ServiceLevelCode = x.Tariff.ServiceLevel.Code,
                    GeoZoneCode = x.Tariff.GeoZone.Code,
                    FromValue = x.FromValue,
                    ToValue = x.ToValue,
                    Price = x.Price,
                    MinCharge = x.MinCharge,
                    StepValue = x.StepValue,
                    SortOrder = x.SortOrder
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SurchargeListItemViewModel>> GetSurchargesAsync()
        {
            return await _dbContext.Surcharges
                .AsNoTracking()
                .OrderBy(x => x.Code)
                .Select(x => new SurchargeListItemViewModel
                {
                    Id = x.Id,
                    Code = x.Code,
                    Name = x.Name,
                    SurchargeType = x.SurchargeType.ToString(),
                    Value = x.Value,
                    Description = x.Description,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TariffSurchargeListItemViewModel>> GetTariffSurchargesAsync()
        {
            return await _dbContext.TariffSurcharges
                .AsNoTracking()
                .OrderBy(x => x.Tariff.ServiceLevel.Code)
                .ThenBy(x => x.Tariff.GeoZone.Code)
                .ThenBy(x => x.Surcharge.Code)
                .Select(x => new TariffSurchargeListItemViewModel
                {
                    Id = x.Id,
                    TariffId = x.TariffId,
                    ServiceLevelCode = x.Tariff.ServiceLevel.Code,
                    GeoZoneCode = x.Tariff.GeoZone.Code,
                    SurchargeCode = x.Surcharge.Code,
                    ApplyAs = x.ApplyAs.ToString(),
                    Value = x.Value,
                    MinAmount = x.MinAmount,
                    MaxAmount = x.MaxAmount,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AgreementListItemViewModel>> GetAgreementsAsync(Guid companyId)
        {
            return await _dbContext.Agreements
                .AsNoTracking()
                .Where(x => x.CompanyId == companyId)
                .OrderByDescending(x => x.ValidFromUtc)
                .ThenBy(x => x.AgreementNumber)
                .Select(x => new AgreementListItemViewModel
                {
                    Id = x.Id,
                    AgreementNumber = x.AgreementNumber,
                    AgreementType = x.AgreementType.ToString(),
                    ValidFromUtc = x.ValidFromUtc,
                    ValidToUtc = x.ValidToUtc,
                    Currency = x.Currency,
                    PaymentTerms = x.PaymentTerms,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DiscountRuleListItemViewModel>> GetDiscountRulesAsync(Guid companyId)
        {
            return await _dbContext.DiscountRules
                .AsNoTracking()
                .Where(x => x.Agreement.CompanyId == companyId)
                .OrderBy(x => x.Agreement.AgreementNumber)
                .ThenBy(x => x.ServiceLevel != null ? x.ServiceLevel.Code : string.Empty)
                .ThenBy(x => x.GeoZone != null ? x.GeoZone.Code : string.Empty)
                .Select(x => new DiscountRuleListItemViewModel
                {
                    Id = x.Id,
                    AgreementNumber = x.Agreement.AgreementNumber,
                    ServiceLevelCode = x.ServiceLevel != null ? x.ServiceLevel.Code : null,
                    GeoZoneCode = x.GeoZone != null ? x.GeoZone.Code : null,
                    DiscountType = x.DiscountType.ToString(),
                    Value = x.Value,
                    MinShipmentValue = x.MinShipmentValue,
                    MaxShipmentValue = x.MaxShipmentValue,
                    ValidFromUtc = x.ValidFromUtc,
                    ValidToUtc = x.ValidToUtc,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PricingQuoteListItemViewModel>> GetPricingQuotesAsync(Guid companyId)
        {
            return await _dbContext.PricingQuotes
                .AsNoTracking()
                .Where(x => x.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ThenByDescending(x => x.ValidUntilUtc)
                .Select(x => new PricingQuoteListItemViewModel
                {
                    Id = x.Id,
                    QuoteNumber = x.QuoteNumber,
                    AgreementNumber = x.Agreement != null ? x.Agreement.AgreementNumber : null,
                    ServiceLevelCode = x.ServiceLevel != null ? x.ServiceLevel.Code : null,
                    Status = x.Status.ToString(),
                    Currency = x.Currency,
                    ValidUntilUtc = x.ValidUntilUtc,
                    NetAmount = x.NetAmount,
                    TaxAmount = x.TaxAmount,
                    TotalAmount = x.TotalAmount,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PricingQuoteLineListItemViewModel>> GetPricingQuoteLinesAsync(Guid companyId)
        {
            return await _dbContext.PricingQuoteLines
                .AsNoTracking()
                .Where(x => x.PricingQuote.CustomerCompanyId == companyId)
                .OrderBy(x => x.PricingQuote.QuoteNumber)
                .ThenBy(x => x.LineNo)
                .Select(x => new PricingQuoteLineListItemViewModel
                {
                    Id = x.Id,
                    PricingQuoteId = x.PricingQuoteId,
                    QuoteNumber = x.PricingQuote.QuoteNumber,
                    LineNo = x.LineNo,
                    LineType = x.LineType.ToString(),
                    Description = x.Description,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    LineAmount = x.LineAmount,
                    ReferenceCode = x.ReferenceCode,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        #endregion


        // Billing / Finance

        #region Billing

        public async Task<IEnumerable<ChargeListItemViewModel>> GetChargesAsync(Guid companyId)
        {
            return await _dbContext.Charges
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new ChargeListItemViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentNo = x.Shipment.ShipmentNo,
                    ChargeCode = x.ChargeCode,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Currency = x.Currency,
                    SourceType = x.SourceType.ToString(),
                    IsTaxable = x.IsTaxable,
                    TaxRatePercent = x.TaxRatePercent,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ChargeRuleAppliedListItemViewModel>> GetChargeRulesAppliedAsync(Guid companyId)
        {
            return await _dbContext.ChargeRulesApplied
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Charge.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new ChargeRuleAppliedListItemViewModel
                {
                    Id = x.Id,
                    ChargeId = x.ChargeId,
                    ShipmentNo = x.Charge.Shipment.ShipmentNo,
                    ChargeCode = x.Charge.ChargeCode,
                    SourceEntityType = x.SourceEntityType,
                    SourceEntityId = x.SourceEntityId,
                    RuleCode = x.RuleCode,
                    RuleDescription = x.RuleDescription,
                    AppliedAmount = x.AppliedAmount,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<InvoiceListItemViewModel>> GetInvoicesAsync(Guid companyId)
        {
            return await _dbContext.Invoices
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.BillToCompanyId == companyId)
                .OrderByDescending(x => x.IssueDateUtc)
                .ThenByDescending(x => x.CreatedAtUtc)
                .Select(x => new InvoiceListItemViewModel
                {
                    Id = x.Id,
                    InvoiceNo = x.InvoiceNo,
                    IssueDateUtc = x.IssueDateUtc,
                    DueDateUtc = x.DueDateUtc,
                    Currency = x.Currency,
                    Status = x.Status.ToString(),
                    SubtotalAmount = x.SubtotalAmount,
                    TaxAmount = x.TaxAmount,
                    TotalAmount = x.TotalAmount,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<InvoiceLineListItemViewModel>> GetInvoiceLinesAsync(Guid companyId)
        {
            return await _dbContext.InvoiceLines
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Invoice.BillToCompanyId == companyId)
                .OrderBy(x => x.Invoice.InvoiceNo)
                .ThenBy(x => x.LineNo)
                .Select(x => new InvoiceLineListItemViewModel
                {
                    Id = x.Id,
                    InvoiceId = x.InvoiceId,
                    InvoiceNo = x.Invoice.InvoiceNo,
                    ShipmentNo = x.Shipment != null ? x.Shipment.ShipmentNo : null,
                    ChargeCode = x.Charge != null ? x.Charge.ChargeCode : null,
                    LineNo = x.LineNo,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    TaxRatePercent = x.TaxRatePercent,
                    LineNetAmount = x.LineNetAmount,
                    LineTaxAmount = x.LineTaxAmount,
                    LineTotalAmount = x.LineTotalAmount
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentListItemViewModel>> GetPaymentsAsync(Guid companyId)
        {
            return await _dbContext.Payments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Invoice.BillToCompanyId == companyId)
                .OrderByDescending(x => x.PaymentDateUtc)
                .ThenByDescending(x => x.CreatedAtUtc)
                .Select(x => new PaymentListItemViewModel
                {
                    Id = x.Id,
                    InvoiceId = x.InvoiceId,
                    InvoiceNo = x.Invoice.InvoiceNo,
                    PaymentDateUtc = x.PaymentDateUtc,
                    Amount = x.Amount,
                    Currency = x.Currency,
                    PaymentMethod = x.PaymentMethod.ToString(),
                    Status = x.Status.ToString(),
                    TransactionReference = x.TransactionReference,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentAllocationListItemViewModel>> GetPaymentAllocationsAsync(Guid companyId)
        {
            return await _dbContext.PaymentAllocations
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Invoice.BillToCompanyId == companyId && x.Payment.Invoice.BillToCompanyId == companyId)
                .OrderByDescending(x => x.AllocatedAtUtc)
                .ThenByDescending(x => x.CreatedAtUtc)
                .Select(x => new PaymentAllocationListItemViewModel
                {
                    Id = x.Id,
                    PaymentId = x.PaymentId,
                    InvoiceId = x.InvoiceId,
                    PaymentInvoiceNo = x.Payment.Invoice.InvoiceNo,
                    InvoiceNo = x.Invoice.InvoiceNo,
                    AllocatedAmount = x.AllocatedAmount,
                    AllocatedAtUtc = x.AllocatedAtUtc,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CreditNoteListItemViewModel>> GetCreditNotesAsync(Guid companyId)
        {
            return await _dbContext.CreditNotes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.BillToCompanyId == companyId)
                .OrderByDescending(x => x.IssueDateUtc)
                .ThenByDescending(x => x.CreatedAtUtc)
                .Select(x => new CreditNoteListItemViewModel
                {
                    Id = x.Id,
                    InvoiceId = x.InvoiceId,
                    CreditNoteNo = x.CreditNoteNo,
                    InvoiceNo = x.Invoice.InvoiceNo,
                    IssueDateUtc = x.IssueDateUtc,
                    Currency = x.Currency,
                    Status = x.Status.ToString(),
                    NetAmount = x.NetAmount,
                    TaxAmount = x.TaxAmount,
                    TotalAmount = x.TotalAmount,
                    Reason = x.Reason,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TaxRateListItemViewModel>> GetTaxRatesAsync()
        {
            return await _dbContext.TaxRates
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.CountryCode)
                .ThenBy(x => x.Name)
                .Select(x => new TaxRateListItemViewModel
                {
                    Id = x.Id,
                    TaxType = x.TaxType.ToString(),
                    Name = x.Name,
                    CountryCode = x.CountryCode,
                    RatePercent = x.RatePercent,
                    ValidFromUtc = x.ValidFromUtc,
                    ValidToUtc = x.ValidToUtc,
                    IsActive = x.IsActive,
                    Notes = x.Notes
                })
                .ToListAsync();
        }

        public async Task<ChargeCreateViewModel> GetCreateChargeModelAsync(Guid companyId)
        {
            var model = new ChargeCreateViewModel();
            await PopulateChargeOptionsAsync(companyId, model);
            return model;
        }

        public async Task<ChargeEditViewModel?> GetChargeForEditAsync(Guid companyId, Guid chargeId)
        {
            var charge = await _dbContext.Charges
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == chargeId && x.Shipment.CustomerCompanyId == companyId)
                .Select(x => new ChargeEditViewModel
                {
                    Id = x.Id,
                    ShipmentId = x.ShipmentId,
                    ShipmentLegId = x.ShipmentLegId,
                    ChargeCode = x.ChargeCode,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Currency = x.Currency,
                    SourceType = x.SourceType,
                    IsTaxable = x.IsTaxable,
                    TaxRatePercent = x.TaxRatePercent,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (charge == null) return null;
            await PopulateChargeOptionsAsync(companyId, charge);
            return charge;
        }

        public async Task<Guid?> CreateChargeAsync(Guid companyId, ChargeCreateViewModel model)
        {
            var shipmentExists = await _dbContext.Shipments.AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId && x.CustomerCompanyId == companyId);
            if (!shipmentExists) return null;

            if (model.ShipmentLegId.HasValue)
            {
                var legExists = await _dbContext.ShipmentLegs.AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentLegId.Value && x.Shipment.CustomerCompanyId == companyId && x.ShipmentId == model.ShipmentId);
                if (!legExists) return null;
            }

            var entity = new Charge
            {
                ShipmentId = model.ShipmentId,
                ShipmentLegId = model.ShipmentLegId,
                ChargeCode = model.ChargeCode.Trim(),
                Description = model.Description.Trim(),
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                Currency = model.Currency.Trim().ToUpperInvariant(),
                SourceType = model.SourceType,
                IsTaxable = model.IsTaxable,
                TaxRatePercent = model.TaxRatePercent,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _dbContext.Charges.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateChargeAsync(Guid companyId, ChargeEditViewModel model)
        {
            var entity = await _dbContext.Charges
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Shipment.CustomerCompanyId == companyId);

            if (entity == null) return false;

            var shipmentExists = await _dbContext.Shipments.AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId && x.CustomerCompanyId == companyId);
            if (!shipmentExists) return false;

            if (model.ShipmentLegId.HasValue)
            {
                var legExists = await _dbContext.ShipmentLegs.AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentLegId.Value && x.Shipment.CustomerCompanyId == companyId && x.ShipmentId == model.ShipmentId);
                if (!legExists) return false;
            }

            entity.ShipmentId = model.ShipmentId;
            entity.ShipmentLegId = model.ShipmentLegId;
            entity.ChargeCode = model.ChargeCode.Trim();
            entity.Description = model.Description.Trim();
            entity.Quantity = model.Quantity;
            entity.UnitPrice = model.UnitPrice;
            entity.Currency = model.Currency.Trim().ToUpperInvariant();
            entity.SourceType = model.SourceType;
            entity.IsTaxable = model.IsTaxable;
            entity.TaxRatePercent = model.TaxRatePercent;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteChargeAsync(Guid companyId, Guid chargeId)
        {
            var entity = await _dbContext.Charges
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == chargeId && x.Shipment.CustomerCompanyId == companyId);

            if (entity == null) return false;
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ChargeRuleAppliedCreateViewModel> GetCreateChargeRuleAppliedModelAsync(Guid companyId)
        {
            var model = new ChargeRuleAppliedCreateViewModel();
            await PopulateChargeRuleAppliedOptionsAsync(companyId, model);
            return model;
        }

        public async Task<ChargeRuleAppliedEditViewModel?> GetChargeRuleAppliedForEditAsync(Guid companyId, Guid chargeRuleAppliedId)
        {
            var item = await _dbContext.ChargeRulesApplied
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == chargeRuleAppliedId && x.Charge.Shipment.CustomerCompanyId == companyId)
                .Select(x => new ChargeRuleAppliedEditViewModel
                {
                    Id = x.Id,
                    ChargeId = x.ChargeId,
                    SourceEntityType = x.SourceEntityType,
                    SourceEntityId = x.SourceEntityId,
                    RuleCode = x.RuleCode,
                    RuleDescription = x.RuleDescription,
                    AppliedAmount = x.AppliedAmount,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (item == null) return null;
            await PopulateChargeRuleAppliedOptionsAsync(companyId, item);
            return item;
        }

        public async Task<Guid?> CreateChargeRuleAppliedAsync(Guid companyId, ChargeRuleAppliedCreateViewModel model)
        {
            var chargeExists = await _dbContext.Charges.AnyAsync(x => !x.IsDeleted && x.Id == model.ChargeId && x.Shipment.CustomerCompanyId == companyId);
            if (!chargeExists) return null;

            var entity = new ChargeRuleApplied
            {
                ChargeId = model.ChargeId,
                SourceEntityType = string.IsNullOrWhiteSpace(model.SourceEntityType) ? null : model.SourceEntityType.Trim(),
                SourceEntityId = model.SourceEntityId,
                RuleCode = string.IsNullOrWhiteSpace(model.RuleCode) ? null : model.RuleCode.Trim(),
                RuleDescription = string.IsNullOrWhiteSpace(model.RuleDescription) ? null : model.RuleDescription.Trim(),
                AppliedAmount = model.AppliedAmount,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _dbContext.ChargeRulesApplied.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateChargeRuleAppliedAsync(Guid companyId, ChargeRuleAppliedEditViewModel model)
        {
            var entity = await _dbContext.ChargeRulesApplied
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Charge.Shipment.CustomerCompanyId == companyId);

            if (entity == null) return false;

            var chargeExists = await _dbContext.Charges.AnyAsync(x => !x.IsDeleted && x.Id == model.ChargeId && x.Shipment.CustomerCompanyId == companyId);
            if (!chargeExists) return false;

            entity.ChargeId = model.ChargeId;
            entity.SourceEntityType = string.IsNullOrWhiteSpace(model.SourceEntityType) ? null : model.SourceEntityType.Trim();
            entity.SourceEntityId = model.SourceEntityId;
            entity.RuleCode = string.IsNullOrWhiteSpace(model.RuleCode) ? null : model.RuleCode.Trim();
            entity.RuleDescription = string.IsNullOrWhiteSpace(model.RuleDescription) ? null : model.RuleDescription.Trim();
            entity.AppliedAmount = model.AppliedAmount;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteChargeRuleAppliedAsync(Guid companyId, Guid chargeRuleAppliedId)
        {
            var entity = await _dbContext.ChargeRulesApplied
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == chargeRuleAppliedId && x.Charge.Shipment.CustomerCompanyId == companyId);

            if (entity == null) return false;
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<InvoiceEditViewModel?> GetInvoiceForEditAsync(Guid companyId, Guid invoiceId)
        {
            return await _dbContext.Invoices
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == invoiceId && x.BillToCompanyId == companyId)
                .Select(x => new InvoiceEditViewModel
                {
                    Id = x.Id,
                    InvoiceNo = x.InvoiceNo,
                    IssueDateUtc = x.IssueDateUtc,
                    DueDateUtc = x.DueDateUtc,
                    Currency = x.Currency,
                    Status = x.Status,
                    SubtotalAmount = x.SubtotalAmount,
                    TaxAmount = x.TaxAmount,
                    TotalAmount = x.TotalAmount,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Guid?> CreateInvoiceAsync(Guid companyId, InvoiceCreateViewModel model)
        {
            var entity = new Invoice
            {
                InvoiceNo = model.InvoiceNo.Trim(),
                BillToCompanyId = companyId,
                IssueDateUtc = model.IssueDateUtc,
                DueDateUtc = model.DueDateUtc,
                Currency = model.Currency.Trim().ToUpperInvariant(),
                Status = model.Status,
                SubtotalAmount = model.SubtotalAmount,
                TaxAmount = model.TaxAmount,
                TotalAmount = model.TotalAmount,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _dbContext.Invoices.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateInvoiceAsync(Guid companyId, InvoiceEditViewModel model)
        {
            var entity = await _dbContext.Invoices.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.BillToCompanyId == companyId);
            if (entity == null) return false;

            entity.InvoiceNo = model.InvoiceNo.Trim();
            entity.IssueDateUtc = model.IssueDateUtc;
            entity.DueDateUtc = model.DueDateUtc;
            entity.Currency = model.Currency.Trim().ToUpperInvariant();
            entity.Status = model.Status;
            entity.SubtotalAmount = model.SubtotalAmount;
            entity.TaxAmount = model.TaxAmount;
            entity.TotalAmount = model.TotalAmount;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInvoiceAsync(Guid companyId, Guid invoiceId)
        {
            var entity = await _dbContext.Invoices.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == invoiceId && x.BillToCompanyId == companyId);
            if (entity == null) return false;
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<InvoiceLineCreateViewModel> GetCreateInvoiceLineModelAsync(Guid companyId)
        {
            var model = new InvoiceLineCreateViewModel();
            await PopulateInvoiceLineOptionsAsync(companyId, model);
            return model;
        }

        public async Task<InvoiceLineEditViewModel?> GetInvoiceLineForEditAsync(Guid companyId, Guid invoiceLineId)
        {
            var item = await _dbContext.InvoiceLines
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == invoiceLineId && x.Invoice.BillToCompanyId == companyId)
                .Select(x => new InvoiceLineEditViewModel
                {
                    Id = x.Id,
                    InvoiceId = x.InvoiceId,
                    ChargeId = x.ChargeId,
                    ShipmentId = x.ShipmentId,
                    LineNo = x.LineNo,
                    Description = x.Description,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    TaxRatePercent = x.TaxRatePercent,
                    LineNetAmount = x.LineNetAmount,
                    LineTaxAmount = x.LineTaxAmount,
                    LineTotalAmount = x.LineTotalAmount
                })
                .FirstOrDefaultAsync();

            if (item == null) return null;
            await PopulateInvoiceLineOptionsAsync(companyId, item);
            return item;
        }

        public async Task<Guid?> CreateInvoiceLineAsync(Guid companyId, InvoiceLineCreateViewModel model)
        {
            var invoiceExists = await _dbContext.Invoices.AnyAsync(x => !x.IsDeleted && x.Id == model.InvoiceId && x.BillToCompanyId == companyId);
            if (!invoiceExists) return null;

            if (model.ChargeId.HasValue)
            {
                var chargeExists = await _dbContext.Charges.AnyAsync(x => !x.IsDeleted && x.Id == model.ChargeId.Value && x.Shipment.CustomerCompanyId == companyId);
                if (!chargeExists) return null;
            }

            if (model.ShipmentId.HasValue)
            {
                var shipmentExists = await _dbContext.Shipments.AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId.Value && x.CustomerCompanyId == companyId);
                if (!shipmentExists) return null;
            }

            var entity = new InvoiceLine
            {
                InvoiceId = model.InvoiceId,
                ChargeId = model.ChargeId,
                ShipmentId = model.ShipmentId,
                LineNo = model.LineNo,
                Description = model.Description.Trim(),
                Quantity = model.Quantity,
                UnitPrice = model.UnitPrice,
                TaxRatePercent = model.TaxRatePercent,
                LineNetAmount = model.LineNetAmount,
                LineTaxAmount = model.LineTaxAmount,
                LineTotalAmount = model.LineTotalAmount
            };

            _dbContext.InvoiceLines.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateInvoiceLineAsync(Guid companyId, InvoiceLineEditViewModel model)
        {
            var entity = await _dbContext.InvoiceLines
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Invoice.BillToCompanyId == companyId);
            if (entity == null) return false;

            var invoiceExists = await _dbContext.Invoices.AnyAsync(x => !x.IsDeleted && x.Id == model.InvoiceId && x.BillToCompanyId == companyId);
            if (!invoiceExists) return false;

            if (model.ChargeId.HasValue)
            {
                var chargeExists = await _dbContext.Charges.AnyAsync(x => !x.IsDeleted && x.Id == model.ChargeId.Value && x.Shipment.CustomerCompanyId == companyId);
                if (!chargeExists) return false;
            }

            if (model.ShipmentId.HasValue)
            {
                var shipmentExists = await _dbContext.Shipments.AnyAsync(x => !x.IsDeleted && x.Id == model.ShipmentId.Value && x.CustomerCompanyId == companyId);
                if (!shipmentExists) return false;
            }

            entity.InvoiceId = model.InvoiceId;
            entity.ChargeId = model.ChargeId;
            entity.ShipmentId = model.ShipmentId;
            entity.LineNo = model.LineNo;
            entity.Description = model.Description.Trim();
            entity.Quantity = model.Quantity;
            entity.UnitPrice = model.UnitPrice;
            entity.TaxRatePercent = model.TaxRatePercent;
            entity.LineNetAmount = model.LineNetAmount;
            entity.LineTaxAmount = model.LineTaxAmount;
            entity.LineTotalAmount = model.LineTotalAmount;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInvoiceLineAsync(Guid companyId, Guid invoiceLineId)
        {
            var entity = await _dbContext.InvoiceLines
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == invoiceLineId && x.Invoice.BillToCompanyId == companyId);
            if (entity == null) return false;
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<PaymentCreateViewModel> GetCreatePaymentModelAsync(Guid companyId)
        {
            var model = new PaymentCreateViewModel();
            await PopulatePaymentOptionsAsync(companyId, model);
            return model;
        }

        public async Task<PaymentEditViewModel?> GetPaymentForEditAsync(Guid companyId, Guid paymentId)
        {
            var item = await _dbContext.Payments
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == paymentId && x.Invoice.BillToCompanyId == companyId)
                .Select(x => new PaymentEditViewModel
                {
                    Id = x.Id,
                    InvoiceId = x.InvoiceId,
                    PaymentDateUtc = x.PaymentDateUtc,
                    Amount = x.Amount,
                    Currency = x.Currency,
                    PaymentMethod = x.PaymentMethod,
                    Status = x.Status,
                    TransactionReference = x.TransactionReference,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (item == null) return null;
            await PopulatePaymentOptionsAsync(companyId, item);
            return item;
        }

        public async Task<Guid?> CreatePaymentAsync(Guid companyId, PaymentCreateViewModel model)
        {
            var invoiceExists = await _dbContext.Invoices.AnyAsync(x => !x.IsDeleted && x.Id == model.InvoiceId && x.BillToCompanyId == companyId);
            if (!invoiceExists) return null;

            var entity = new Payment
            {
                InvoiceId = model.InvoiceId,
                PaymentDateUtc = model.PaymentDateUtc,
                Amount = model.Amount,
                Currency = model.Currency.Trim().ToUpperInvariant(),
                PaymentMethod = model.PaymentMethod,
                Status = model.Status,
                TransactionReference = string.IsNullOrWhiteSpace(model.TransactionReference) ? null : model.TransactionReference.Trim(),
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _dbContext.Payments.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdatePaymentAsync(Guid companyId, PaymentEditViewModel model)
        {
            var entity = await _dbContext.Payments
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Invoice.BillToCompanyId == companyId);
            if (entity == null) return false;

            var invoiceExists = await _dbContext.Invoices.AnyAsync(x => !x.IsDeleted && x.Id == model.InvoiceId && x.BillToCompanyId == companyId);
            if (!invoiceExists) return false;

            entity.InvoiceId = model.InvoiceId;
            entity.PaymentDateUtc = model.PaymentDateUtc;
            entity.Amount = model.Amount;
            entity.Currency = model.Currency.Trim().ToUpperInvariant();
            entity.PaymentMethod = model.PaymentMethod;
            entity.Status = model.Status;
            entity.TransactionReference = string.IsNullOrWhiteSpace(model.TransactionReference) ? null : model.TransactionReference.Trim();
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePaymentAsync(Guid companyId, Guid paymentId)
        {
            var entity = await _dbContext.Payments
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == paymentId && x.Invoice.BillToCompanyId == companyId);
            if (entity == null) return false;
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<PaymentAllocationCreateViewModel> GetCreatePaymentAllocationModelAsync(Guid companyId)
        {
            var model = new PaymentAllocationCreateViewModel();
            await PopulatePaymentAllocationOptionsAsync(companyId, model);
            return model;
        }

        public async Task<PaymentAllocationEditViewModel?> GetPaymentAllocationForEditAsync(Guid companyId, Guid paymentAllocationId)
        {
            var item = await _dbContext.PaymentAllocations
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == paymentAllocationId && x.Invoice.BillToCompanyId == companyId && x.Payment.Invoice.BillToCompanyId == companyId)
                .Select(x => new PaymentAllocationEditViewModel
                {
                    Id = x.Id,
                    PaymentId = x.PaymentId,
                    InvoiceId = x.InvoiceId,
                    AllocatedAmount = x.AllocatedAmount,
                    AllocatedAtUtc = x.AllocatedAtUtc,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (item == null) return null;
            await PopulatePaymentAllocationOptionsAsync(companyId, item);
            return item;
        }

        public async Task<Guid?> CreatePaymentAllocationAsync(Guid companyId, PaymentAllocationCreateViewModel model)
        {
            var paymentExists = await _dbContext.Payments.AnyAsync(x => !x.IsDeleted && x.Id == model.PaymentId && x.Invoice.BillToCompanyId == companyId);
            var invoiceExists = await _dbContext.Invoices.AnyAsync(x => !x.IsDeleted && x.Id == model.InvoiceId && x.BillToCompanyId == companyId);
            if (!paymentExists || !invoiceExists) return null;

            var entity = new PaymentAllocation
            {
                PaymentId = model.PaymentId,
                InvoiceId = model.InvoiceId,
                AllocatedAmount = model.AllocatedAmount,
                AllocatedAtUtc = model.AllocatedAtUtc,
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _dbContext.PaymentAllocations.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdatePaymentAllocationAsync(Guid companyId, PaymentAllocationEditViewModel model)
        {
            var entity = await _dbContext.PaymentAllocations
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.Invoice.BillToCompanyId == companyId && x.Payment.Invoice.BillToCompanyId == companyId);
            if (entity == null) return false;

            var paymentExists = await _dbContext.Payments.AnyAsync(x => !x.IsDeleted && x.Id == model.PaymentId && x.Invoice.BillToCompanyId == companyId);
            var invoiceExists = await _dbContext.Invoices.AnyAsync(x => !x.IsDeleted && x.Id == model.InvoiceId && x.BillToCompanyId == companyId);
            if (!paymentExists || !invoiceExists) return false;

            entity.PaymentId = model.PaymentId;
            entity.InvoiceId = model.InvoiceId;
            entity.AllocatedAmount = model.AllocatedAmount;
            entity.AllocatedAtUtc = model.AllocatedAtUtc;
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePaymentAllocationAsync(Guid companyId, Guid paymentAllocationId)
        {
            var entity = await _dbContext.PaymentAllocations
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == paymentAllocationId && x.Invoice.BillToCompanyId == companyId && x.Payment.Invoice.BillToCompanyId == companyId);
            if (entity == null) return false;
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<CreditNoteCreateViewModel> GetCreateCreditNoteModelAsync(Guid companyId)
        {
            var model = new CreditNoteCreateViewModel();
            await PopulateCreditNoteOptionsAsync(companyId, model);
            return model;
        }

        public async Task<CreditNoteEditViewModel?> GetCreditNoteForEditAsync(Guid companyId, Guid creditNoteId)
        {
            var item = await _dbContext.CreditNotes
                .AsNoTracking()
                .Where(x => !x.IsDeleted && x.Id == creditNoteId && x.BillToCompanyId == companyId)
                .Select(x => new CreditNoteEditViewModel
                {
                    Id = x.Id,
                    CreditNoteNo = x.CreditNoteNo,
                    InvoiceId = x.InvoiceId,
                    IssueDateUtc = x.IssueDateUtc,
                    Currency = x.Currency,
                    Status = x.Status,
                    NetAmount = x.NetAmount,
                    TaxAmount = x.TaxAmount,
                    TotalAmount = x.TotalAmount,
                    Reason = x.Reason,
                    Notes = x.Notes
                })
                .FirstOrDefaultAsync();

            if (item == null) return null;
            await PopulateCreditNoteOptionsAsync(companyId, item);
            return item;
        }

        public async Task<Guid?> CreateCreditNoteAsync(Guid companyId, CreditNoteCreateViewModel model)
        {
            var invoiceExists = await _dbContext.Invoices.AnyAsync(x => !x.IsDeleted && x.Id == model.InvoiceId && x.BillToCompanyId == companyId);
            if (!invoiceExists) return null;

            var entity = new CreditNote
            {
                CreditNoteNo = model.CreditNoteNo.Trim(),
                InvoiceId = model.InvoiceId,
                BillToCompanyId = companyId,
                IssueDateUtc = model.IssueDateUtc,
                Currency = model.Currency.Trim().ToUpperInvariant(),
                Status = model.Status,
                NetAmount = model.NetAmount,
                TaxAmount = model.TaxAmount,
                TotalAmount = model.TotalAmount,
                Reason = string.IsNullOrWhiteSpace(model.Reason) ? null : model.Reason.Trim(),
                Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim()
            };

            _dbContext.CreditNotes.Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateCreditNoteAsync(Guid companyId, CreditNoteEditViewModel model)
        {
            var entity = await _dbContext.CreditNotes.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == model.Id && x.BillToCompanyId == companyId);
            if (entity == null) return false;

            var invoiceExists = await _dbContext.Invoices.AnyAsync(x => !x.IsDeleted && x.Id == model.InvoiceId && x.BillToCompanyId == companyId);
            if (!invoiceExists) return false;

            entity.CreditNoteNo = model.CreditNoteNo.Trim();
            entity.InvoiceId = model.InvoiceId;
            entity.IssueDateUtc = model.IssueDateUtc;
            entity.Currency = model.Currency.Trim().ToUpperInvariant();
            entity.Status = model.Status;
            entity.NetAmount = model.NetAmount;
            entity.TaxAmount = model.TaxAmount;
            entity.TotalAmount = model.TotalAmount;
            entity.Reason = string.IsNullOrWhiteSpace(model.Reason) ? null : model.Reason.Trim();
            entity.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCreditNoteAsync(Guid companyId, Guid creditNoteId)
        {
            var entity = await _dbContext.CreditNotes.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == creditNoteId && x.BillToCompanyId == companyId);
            if (entity == null) return false;
            entity.IsDeleted = true;
            entity.DeletedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private async Task PopulateChargeOptionsAsync(Guid companyId, ChargeCreateViewModel model)
        {
            model.ShipmentOptions = await _dbContext.Shipments.AsNoTracking()
                .Where(x => !x.IsDeleted && x.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ShipmentNo })
                .ToListAsync();

            model.ShipmentLegOptions = await _dbContext.ShipmentLegs.AsNoTracking()
                .Where(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.LegNo.ToString() })
                .ToListAsync();
        }

        private async Task PopulateChargeRuleAppliedOptionsAsync(Guid companyId, ChargeRuleAppliedCreateViewModel model)
        {
            model.ChargeOptions = await _dbContext.Charges.AsNoTracking()
                .Where(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.Shipment.ShipmentNo} | {x.ChargeCode} | {x.Description}" })
                .ToListAsync();
        }

        private async Task PopulateInvoiceLineOptionsAsync(Guid companyId, InvoiceLineCreateViewModel model)
        {
            model.InvoiceOptions = await _dbContext.Invoices.AsNoTracking()
                .Where(x => !x.IsDeleted && x.BillToCompanyId == companyId)
                .OrderByDescending(x => x.IssueDateUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.InvoiceNo })
                .ToListAsync();

            model.ChargeOptions = await _dbContext.Charges.AsNoTracking()
                .Where(x => !x.IsDeleted && x.Shipment.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.Shipment.ShipmentNo} | {x.ChargeCode}" })
                .ToListAsync();

            model.ShipmentOptions = await _dbContext.Shipments.AsNoTracking()
                .Where(x => !x.IsDeleted && x.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ShipmentNo })
                .ToListAsync();
        }

        private async Task PopulatePaymentOptionsAsync(Guid companyId, PaymentCreateViewModel model)
        {
            model.InvoiceOptions = await _dbContext.Invoices.AsNoTracking()
                .Where(x => !x.IsDeleted && x.BillToCompanyId == companyId)
                .OrderByDescending(x => x.IssueDateUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.InvoiceNo })
                .ToListAsync();
        }

        private async Task PopulatePaymentAllocationOptionsAsync(Guid companyId, PaymentAllocationCreateViewModel model)
        {
            model.PaymentOptions = await _dbContext.Payments.AsNoTracking()
                .Where(x => !x.IsDeleted && x.Invoice.BillToCompanyId == companyId)
                .OrderByDescending(x => x.PaymentDateUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.Invoice.InvoiceNo} | {x.PaymentDateUtc:dd.MM.yyyy} | {x.Amount:F2} {x.Currency}" })
                .ToListAsync();

            model.InvoiceOptions = await _dbContext.Invoices.AsNoTracking()
                .Where(x => !x.IsDeleted && x.BillToCompanyId == companyId)
                .OrderByDescending(x => x.IssueDateUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.InvoiceNo })
                .ToListAsync();
        }

        private async Task PopulateCreditNoteOptionsAsync(Guid companyId, CreditNoteCreateViewModel model)
        {
            model.InvoiceOptions = await _dbContext.Invoices.AsNoTracking()
                .Where(x => !x.IsDeleted && x.BillToCompanyId == companyId)
                .OrderByDescending(x => x.IssueDateUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.InvoiceNo })
                .ToListAsync();
        }

        #endregion

        //Status History

        #region Status history

        public async Task<CompanyShipmentStatusHistoryViewModel?> GetShipmentStatusHistoryAsync(Guid companyId, Guid shipmentId)
        {
            var shipment = await GetCompanyAccessibleShipments(companyId)
                .AsNoTracking()
                .Where(x => x.Id == shipmentId)
                .Select(x => new
                {
                    x.Id,
                    x.ShipmentNo
                })
                .FirstOrDefaultAsync();

            if (shipment == null)
            {
                return null;
            }

            var items = await _dbContext.ShipmentStatusHistories
                .AsNoTracking()
                .Where(x => x.ShipmentId == shipmentId)
                .OrderByDescending(x => x.ChangedAtUtc)
                .Select(x => new CompanyShipmentStatusHistoryItemViewModel
                {
                    OldStatus = x.OldStatus.ToString(),
                    NewStatus = x.NewStatus.ToString(),
                    ChangedAtUtc = x.ChangedAtUtc,
                    Reason = x.Reason
                })
                .ToListAsync();

            return new CompanyShipmentStatusHistoryViewModel
            {
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                Items = items
            };
        }

        private async Task PopulateDiscountRuleOptionsAsync(Guid companyId, DiscountRuleCreateViewModel model)
        {
            model.AgreementOptions = await _dbContext.Agreements.AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.AgreementNumber)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.AgreementNumber })
                .ToListAsync();

            model.ServiceLevelOptions = await _dbContext.ServiceLevels.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Code)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.Code} | {x.Name}" })
                .ToListAsync();

            model.GeoZoneOptions = await _dbContext.GeoZones.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Code)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.Code} | {x.Name}" })
                .ToListAsync();
        }

        private async Task PopulatePricingQuoteOptionsAsync(Guid companyId, PricingQuoteCreateViewModel model)
        {
            model.AgreementOptions = await _dbContext.Agreements.AsNoTracking()
                .Where(x => !x.IsDeleted && x.CompanyId == companyId)
                .OrderBy(x => x.AgreementNumber)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.AgreementNumber })
                .ToListAsync();

            model.OrderOptions = await _dbContext.Orders.AsNoTracking()
                .Where(x => !x.IsDeleted && x.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.OrderNo })
                .ToListAsync();

            model.ShipmentOptions = await _dbContext.Shipments.AsNoTracking()
                .Where(x => !x.IsDeleted && x.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.ShipmentNo })
                .ToListAsync();

            model.ServiceLevelOptions = await _dbContext.ServiceLevels.AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderBy(x => x.Code)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.Code} | {x.Name}" })
                .ToListAsync();
        }

        private async Task PopulatePricingQuoteLineOptionsAsync(Guid companyId, PricingQuoteLineCreateViewModel model)
        {
            model.PricingQuoteOptions = await _dbContext.PricingQuotes.AsNoTracking()
                .Where(x => !x.IsDeleted && x.CustomerCompanyId == companyId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.QuoteNumber })
                .ToListAsync();
        }

        #endregion
    }
}

