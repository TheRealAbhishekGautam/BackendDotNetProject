using System;
namespace MyProject0.Utility
{
	public static class SD
	{
        // A Customer is a basic Customer who registers for an account and places the order by making the payment rightaway
		public const string Role_Customer = "Customer";

        // Another type of users are Company Users who doesnot pay the price of the order rightaway,
        // instead they get a period of 30 day to make the payment of the order which is called as Net30.
        // So we need another Controller of company to do such complex orders.
        public const string Role_Company = "Company";

        // Admin is the only one who can make the CRUD operation to the database for all the Products, Catagories and other Content Management.
        public const string Role_Admin = "Admin";

        // Employee will have the access to modify the shipment details and order details.
        public const string Role_Employee = "Employee";

        // These all are the combinations that we can have for Tracking the order for a Regular Customer and a Corporate Customer.
		public const string StatusPeding = "Pending";
		public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "ApprovedForDelayedPayment";
        public const string PaymentStatusRejected = "Rejected";

        // Adding the key of the variable which will store the count of number of items inside the cart using sessions
        public const string SessionCart = "SessionShoppingCart";
    }
}

