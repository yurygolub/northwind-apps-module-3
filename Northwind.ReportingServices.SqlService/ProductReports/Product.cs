namespace Northwind.ReportingServices.SqlService.ProductReports
{
    /// <summary>
    /// Product.
    /// </summary>
    internal class Product
    {
        /// <summary>
        /// Gets or sets productID.
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Gets or sets productName.
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets supplierID.
        /// </summary>
        public int SupplierID { get; set; }

        /// <summary>
        /// Gets or sets categoryID.
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets or sets quantityPerUnit.
        /// </summary>
        public string QuantityPerUnit { get; set; }

        /// <summary>
        /// Gets or sets unitPrice.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets unitsInStock.
        /// </summary>
        public short UnitsInStock { get; set; }

        /// <summary>
        /// Gets or sets unitsOnOrder.
        /// </summary>
        public short UnitsOnOrder { get; set; }

        /// <summary>
        /// Gets or sets reorderLevel.
        /// </summary>
        public short ReorderLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether discontinued.
        /// </summary>
        public bool Discontinued { get; set; }
    }
}
