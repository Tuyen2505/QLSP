﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebQLSP.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class InvoiceOut
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InvoiceOut()
        {
            this.DetailInvoiceOuts = new List<DetailInvoiceOut>();
        }
        [Required(ErrorMessage = "Vui lòng thông tin cho trường này")]
        public string Inv_ID { get; set; }
        [Required(ErrorMessage = "Vui lòng thông tin cho trường này")]
        public System.DateTime Inv_DateOut { get; set; }

        public int Cus_ID { get; set; }
        public bool isDelete { get; set; }
        public Nullable<int> Emp_ID { get; set; }
    
        public virtual Customer Customer { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<DetailInvoiceOut> DetailInvoiceOuts { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
