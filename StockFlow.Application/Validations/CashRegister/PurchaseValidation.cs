using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.CashRegister;

namespace StockFlow.Application.Validations.CashRegister
{
    public static class PurchaseValidation
    {
        public const int MaxAmountPerLine = 999;

        public static void Validate(this Purchase purchase, bool customerExists, int availablePoints, DateTime? movingSaleUntil = null, DateTime? today = null)
        {
            Guard.NotNull(purchase, "Purchase");
            DateTime date = (today ?? DateTime.Today).Date;

            var lines = purchase.ProductsSold?.ToList() ?? [];
            if (lines.Count == 0)
                throw new ArgumentException("A purchase needs at least one product.");

            foreach (var line in lines)
            {
                if (line.Product is null || line.Product.Id == Guid.Empty)
                    throw new ArgumentException("Every line in the purchase needs a product.");

                if (line.Amount < 1 || line.Amount > MaxAmountPerLine)
                    throw new ArgumentException($"The amount of {line.Product.ProductName} must be between 1 and {MaxAmountPerLine}.");

                if (line.PriceAtTimeOfSale < 0)
                    throw new ArgumentException($"The price of {line.Product.ProductName} can't be negative.");

                if (line.Sale is null) continue;

                if (line.Sale.Id == SalePricing.MovingSaleId)
                {
                    if (!SalePricing.IsMovingSaleActive(movingSaleUntil, date))
                        throw new ArgumentException("This customer has no active moving sale.");
                }
                else if (!SalePricing.IsActive(line.Sale, date))
                {
                    throw new ArgumentException($"The sale {line.Sale.SaleName} is not active today.");
                }
            }

            if (purchase.TotalPrice < 0 || purchase.TotalOff < 0)
                throw new ArgumentException("The total and the discount can't be negative.");

            if (purchase.CustomerId is not null && !customerExists)
                throw new ArgumentException("The customer of this purchase doesn't exist.");

            if (purchase.CustomerId is null && lines.Any(l => l.Sale is not null))
                throw new ArgumentException("Sales can only be applied when a customer is added to the purchase.");

            if (purchase.PointsUsed < 0)
                throw new ArgumentException("Points used can't be negative.");

            if (purchase.PointsUsed > 0)
            {
                if (purchase.CustomerId is null)
                    throw new ArgumentException("Points can only be used when a customer is added to the purchase.");

                if (purchase.PointsUsed > availablePoints)
                    throw new ArgumentException($"The customer only has {availablePoints} points.");

                decimal linesTotal = lines.Sum(l => SalePricing.Total(l.PriceAtTimeOfSale, l.Amount));
                if (LoyaltyPoints.ToEuros(purchase.PointsUsed) > linesTotal)
                    throw new ArgumentException("More points are used than the purchase is worth.");

                int redeemable = LoyaltyPoints.Redeemable(availablePoints, linesTotal);
                if (purchase.PointsUsed != redeemable)
                    throw new ArgumentException($"Points are all or nothing: this purchase must use {redeemable} points.");
            }
        }
    }
}
