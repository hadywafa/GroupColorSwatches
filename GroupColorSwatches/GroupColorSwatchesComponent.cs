using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using Rhino.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace GroupColorSwatches
{
    public class GroupColorSwatchesComponent : GH_Component
    {
        public GroupColorSwatchesComponent()
            : base(
                "Group Color Swatches",
                "Auto Color Group",
                "Assign colors to current groups in gh document",
                "Hady Wafa",
                "Groups"
            ) { }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddColourParameter(
                "Target Colors",
                "Colors",
                "Target Colors that you need to assign ti its corresponding Groups.",
                GH_ParamAccess.list
            );

            pManager.AddBooleanParameter(
                "Update",
                "update",
                "update component",
                GH_ParamAccess.item
            );
        }

        protected override void RegisterOutputParams(
            GH_Component.GH_OutputParamManager pManager
        ) { }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var targetColors = new List<Color>();
            bool update = false;
            if (!DA.GetDataList(0, targetColors) || !DA.GetData<bool>(1, ref update))
                return;
            //----------------------------------------------------
            if (!update)
                return;
            IGH_Param inputParameter = Params.Input[0];
            var dataAccessor = inputParameter.Sources;
            //----------------------------------------------------
            var doc = Grasshopper.Instances.ActiveCanvas.Document;
            var currentGroups = doc.Objects.OfType<GH_Group>().ToList();
            var colourSwatches = dataAccessor.OfType<GH_ColourSwatch>().ToList();
            if (colourSwatches.Count == 0 || currentGroups.Count == 0)
                return;
            var options = GetOptionData(colourSwatches);
            var targetGroups = currentGroups.Where(x => !string.IsNullOrEmpty(x.NickName)).ToList();
            foreach (var group in targetGroups)
            {
                if (
                    !string.IsNullOrEmpty(group.NickName)
                    && options.Select(x => x.NickName).Contains(group.NickName)
                )
                {
                    var option = options.Where(x => x.NickName == group.NickName).FirstOrDefault();
                    if (option != null)
                    {
                        group.Colour = option.Color;
                        group.Border = option.Border;
                    }
                    else
                        continue;
                }
            }
            //----------------------------------- Auto Recompute Component Options ------------------------------
            //ExpireSolution(true);
        }

        public List<GroupOptionDto> GetOptionData(List<GH_ColourSwatch> colourSwatches)
        {
            var doc = Grasshopper.Instances.ActiveCanvas.Document;
            var options = new List<GroupOptionDto>();
            foreach (var swatch in colourSwatches)
            {
                var swatchgroup = GetGroupByComponet(swatch);
                if (swatchgroup != null)
                {
                    swatchgroup.Colour = swatch.SwatchColour;
                }
                options.Add(
                    new GroupOptionDto
                    {
                        NickName = swatch.NickName,
                        Color = swatch.SwatchColour,
                        Border = swatchgroup?.Border ?? GH_GroupBorder.Rectangles,
                    }
                );
            }
            return options;
        }

        public GH_Group GetGroupByComponet(GH_ColourSwatch swatch)
        {
            var doc = Grasshopper.Instances.ActiveCanvas.Document;
            var result = doc.Objects
                .OfType<GH_Group>()
                .FirstOrDefault(group => group.Objects().Contains(swatch));
            return result;
        }

        protected override System.Drawing.Bitmap Icon =>
            Properties.Resources.icons8_color_palette.ToBitmap();
        public override Guid ComponentGuid => new Guid("F3485AE6-E12F-4367-BD17-E1297BA16D26");

        // Useless
        public override GH_Exposure Exposure => GH_Exposure.primary;
    }
}

public class GroupOptionDto
{
    public string NickName { get; set; }
    public Color Color { get; set; }
    public GH_GroupBorder Border { get; set; }
}
