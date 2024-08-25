namespace WebQLSP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Customer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Customer()
        {
            this.InvoiceOuts = new HashSet<InvoiceOut>();
        }

        [Required(ErrorMessage = "Vui lòng thông tin cho trường này")]
        public int Cus_ID { get; set; }
        [Required(ErrorMessage = "Vui lòng thông tin cho trường này")]
        public string Cus_Name { get; set; }
        [Required(ErrorMessage = "Vui lòng thông tin cho trường này")]
        public string Cus_Phone { get; set; }

        public bool isDelete { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceOut> InvoiceOuts { get; set; }
    }
}
