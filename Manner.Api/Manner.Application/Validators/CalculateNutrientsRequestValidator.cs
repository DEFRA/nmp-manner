using FluentValidation;
using Manner.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Validators
{
    public class CalculateNutrientsRequestValidator : AbstractValidator<CalculateNutrientsRequest>
    {
        public CalculateNutrientsRequestValidator()
        {

            //RuleFor(x => x.Applications[0].ApplicationRate.Value)
            //    .InclusiveBetween(0,250).WithMessage("Application Rate must be between 0 to 250")
            //    .NotNull().WithMessage("CropTypeId is required.");
            //RuleFor(x => x.ApplicationMonth)
            //    .NotNull().WithMessage("Application Month number is required.")
            //    .InclusiveBetween(1, 12).WithMessage("Month number must be beween 1 to 12.");

            //if (this._blnIsMetric)
            //{
            //    intAppRateMaxValue = 250;
            //}
            //else if (thisManureAppUC.IsManureLiquid)
            //{
            //    intAppRateMaxValue = 22255;
            //}
            //else
            //{
            //    intAppRateMaxValue = 99;
            //}

            //if (MannerNPK.MannerExtrasLib.IsManureCattleSlurry(thisManureAnalysisUC.cboManureType) | MannerNPK.MannerExtrasLib.IsManurePigSlurry(thisManureAnalysisUC.cboManureType))
            //{
            //    isValid = isValid & MannerNPK.Misc.IsTextboxValid(thisManureAnalysisUC.txtDryMatter, "numeric", 0d, 25d);
            //}
            //else
            //{
            //    isValid = isValid & MannerNPK.Misc.IsTextboxValid(thisManureAnalysisUC.txtDryMatter, "numeric", 0d, 99d);
            //}

            // Total N must be equal to or more than the sum of Ammonium-N + Uric acid N + Nitrate N
            //double dblTotalN = Convert.ToDouble(thisManureAnalysisUC.txtTotalN.Text);
            //double dblAmmoniumN = Convert.ToDouble(thisManureAnalysisUC.txtAmmoniumN.Text);
            //double dblUricAcid = Convert.ToDouble(thisManureAnalysisUC.txtUricAcid.Text);
            //double dblNitrateN = Convert.ToDouble(thisManureAnalysisUC.txtNitrateN.Text);
            //if (dblTotalN < dblAmmoniumN + dblUricAcid + dblNitrateN)
            //{
            //    MannerNPK.Misc.InvalidateTextbox(thisManureAnalysisUC.txtTotalN, "must be equal to or more than the sum of Ammonium-N + Uric acid N + Nitrate N");
            //    isValid = false;
            //}

            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(thisManureAnalysisUC.txtTotalN, "numeric", 0d, 297d);
            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(thisManureAnalysisUC.txtAmmoniumN, "numeric", 0d, 99d);
            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(thisManureAnalysisUC.txtUricAcid, "numeric", 0d, 99d);
            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(thisManureAnalysisUC.txtNitrateN, "numeric", 0d, 99d);

            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(thisManureAnalysisUC.txtP, "numeric", 0d, 99d);
            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(thisManureAnalysisUC.txtK, "numeric", 0d, 99d);
            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(thisManureAnalysisUC.txtS, "numeric", 0d, 99d);
            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(thisManureAnalysisUC.txtMg, "numeric", 0d, 99d);

            // prices
            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(this.txtNutrientPrice_N, "numeric");
            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(this.txtNutrientPrice_P2O5, "numeric");
            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(this.txtNutrientPrice_K2O, "numeric");
            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(this.txtNutrientPrice_2_N, "numeric");
            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(this.txtNutrientPrice_2_P2O5, "numeric");
            //isValid = isValid & MannerNPK.Misc.IsTextboxValid(this.txtNutrientPrice_2_K2O, "numeric");


        }
    }
}
