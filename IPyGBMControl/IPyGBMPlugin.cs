﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using IPyModeling;

namespace IPyGBMPlugin
{
    [Export(typeof(VBCommon.Interfaces.IModel))]
    [ExportMetadata("PluginKey", "GBM")]
    class IPyGBMPlugin : IPyModelingPlugin, VBCommon.Interfaces.IModel
    {
        public IPyGBMPlugin()
        {
            base.strPanelKey = "GBMPanel";
            base.strPanelCaption = "GBM";
        }

        public override void Activate()
        {
            innerIronPythonControl = new IPyGBMControl.IPyGBMControl();
            base.Activate();
        }

        public List<double> Predict(System.Data.DataSet dsPredictionData, double RegulatoryThreshold, double DecisionThreshold, VBCommon.Transforms.DependentVariableTransforms ThresholdTransform, double ThresholdPowerExponent) { return (innerIronPythonControl.Predict(dsPredictionData, RegulatoryThreshold, DecisionThreshold, ThresholdTransform, ThresholdPowerExponent)); }
        public List<double> PredictExceedanceProbability(System.Data.DataSet dsPredictionData, double RegulatoryThreshold, double DecisionThreshold, VBCommon.Transforms.DependentVariableTransforms ThresholdTransform, double ThresholdPowerExponent) { return (innerIronPythonControl.PredictExceedanceProbability(dsPredictionData, RegulatoryThreshold, DecisionThreshold, ThresholdTransform, ThresholdPowerExponent)); }
        public string ModelString() { return (innerIronPythonControl.ModelString()); }
    }
}
