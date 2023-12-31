using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject0.Utility
{
    // The values of those varibles can directly be fetched by appsettigs anywhere inside the solution
    // However we are making this class to map those values from here and we will use this class
    // to fetch these values anywhere.

	public class StripeSettings
	{
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}
