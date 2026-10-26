using StockFlow.Application.Interfaces.ManagementApp.Stock;
using StockFlow.Application.Interfaces.Ordering;
using StockFlow.Application.Interfaces.Repositories.ManagementApp.Stock;
using StockFlow.Application.Mappings.Ordering;
using StockFlow.Application.Validations;
using StockFlow.Application.Validations.Stock;
using StockFlow.Domain.Entities.Stock;
using StockFlow.Shared.DTOs.Pagination;
using StockFlow.Shared.DTOs.Stock.Supplier;

namespace StockFlow.Application.Services.ManagementApp.Stock
{
    public class SupplierService(ISupplierRepository supplierRepository, IProductRepository productRepository, IPurchaseOrderService purchaseOrderService) : ISupplierService
    {
        public const string DefaultSupplierCantBeDeleted = "The default supplier can't be deleted.";

        public async Task<List<SupplierDto>> GetAllSuppliersAsync(SupplierPaginationDto pagination)
        {
            pagination.Validate();
            var suppliers = (await supplierRepository.GetAllSuppliersAsync(pagination)).ToList();
            var counts = await supplierRepository.GetProductCountsAsync(suppliers.Select(s => s.Id));
            return suppliers.Select(s => s.ToSupplierDto(DateTime.Today, counts.GetValueOrDefault(s.Id))).ToList();
        }

        public async Task<SupplierDto> CreateSupplierAsync(CreateSupplierDto dto)
        {
            dto.Validate();
            var supplier = dto.ToSupplierEntity();
            await supplierRepository.AddAsync(supplier);
            return supplier.ToSupplierDto(DateTime.Today);
        }

        public async Task<SupplierDto> UpdateSupplierAsync(UpdateSupplierDto dto)
        {
            dto.Validate();
            var supplier = await supplierRepository.GetByIdAsync(dto.Id);
            if (supplier is null || supplier.SoftDeleted)
                throw new KeyNotFoundException("Supplier not found.");

            bool orderDayChanged = supplier.OrderDay != dto.OrderDay;
            supplier.ToSupplierEntity(dto);
            await supplierRepository.UpdateAsync(supplier);
            if (orderDayChanged)
                await purchaseOrderService.RescheduleOpenOrdersAsync(supplier.Id, supplier.OrderDay, DateTime.Today);

            return supplier.ToSupplierDto(DateTime.Today, await productRepository.CountProductsForSupplierAsync(supplier.Id));
        }

        public async Task SoftDeleteSupplierAsync(Guid id)
        {
            if (id == Supplier.DefaultSupplierId)
                throw new ArgumentException(DefaultSupplierCantBeDeleted);

            var supplier = await supplierRepository.GetByIdAsync(id);
            if (supplier is null || supplier.SoftDeleted)
                throw new KeyNotFoundException("Supplier not found.");

            int products = await productRepository.CountProductsForSupplierAsync(id);
            if (products > 0)
                throw new ArgumentException($"{supplier.SupplierName} still supplies {products} product(s). Move them to another supplier first.");

            await supplierRepository.SoftDeleteAsync(supplier);
        }
    }
}
