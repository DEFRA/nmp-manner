using FluentValidation;
using Manner.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manner.Application.Validators
{
    public class MannerApplicationValidator : AbstractValidator<MannerApplication>
    {
        public MannerApplicationValidator()
        {
            //if ((int)MannerApplication.Application.ApplicationMethodEnum == default(int))
            //{
            //    canproc = false;
            //    throw new Exception("Application method not set");
            //}

            //if ((int)MannerApplication.Application.MethodOfIncorporationEnum == default(int))
            //{
            //    canproc = false;
            //    throw new Exception("Method of incorporation not set");
            //}

            //if ((int)MannerApplication.Application.DelayToIncorporationEnum == default(int))
            //{
            //    canproc = false;
            //    throw new Exception("Delay to incorporation not set");
            //}


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
