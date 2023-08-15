using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace Apt_Management_App.ViewModels
{
    internal class Validation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        /*
         * Validates the values inserted in the
         * DTO object before adding them to the database.
         * Gets called automatically by the datagrid as
         * soon as the user presses the enter key.
         */
        {
            var rowEdited = ((BindingGroup)value).Items[0];
            ValidationResult validInput = ValidationResult.ValidResult;

            if (rowEdited != null)
            {
                switch (rowEdited)
                {
                    case Repository.ApartmentDTO:
                        validInput = Repository.ApartmentDTO.ValidInput((Repository.ApartmentDTO)rowEdited);
                        break;
                    case Repository.ContractDTO:
                        validInput = Repository.ContractDTO.ValidInput((Repository.ContractDTO)rowEdited);
                        break;
                    case Repository.ElectricityContractDTO:
                        validInput = Repository.ElectricityContractDTO.ValidInput((Repository.ElectricityContractDTO)rowEdited);
                        break;
                    case Repository.PaymentDTO:
                        validInput = Repository.PaymentDTO.ValidInput((Repository.PaymentDTO)rowEdited);
                        break;
                    case Repository.RenterDTO:
                        validInput = Repository.RenterDTO.ValidInput((Repository.RenterDTO)rowEdited);
                        break;
                    case Repository.WaterContractDTO:
                        validInput = Repository.WaterContractDTO.ValidInput((Repository.WaterContractDTO)rowEdited);
                        break;
                }
            }
            return validInput;
        }
    }
}
