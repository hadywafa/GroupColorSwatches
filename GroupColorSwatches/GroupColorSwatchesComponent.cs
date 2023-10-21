using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;
using Rhino.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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
        }

        protected override void RegisterOutputParams(
            GH_Component.GH_OutputParamManager pManager
        ) { }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var targetColors = new List<Color>();

            if (!DA.GetDataList(0, targetColors))
                return;

            //----------------------------------------------------
            IGH_Param inputParameter = Params.Input[1];
            var dataAccessor = inputParameter.Sources;
            var colourSwatches = dataAccessor.OfType<GH_ColourSwatch>().ToList();
            //----------------------------------------------------
            var options = GetOptionData(colourSwatches);

            var doc = Grasshopper.Instances.ActiveCanvas.Document;
            var currentGroups = doc.Objects.OfType<GH_Group>().ToList();
            foreach (var group in currentGroups)
            {
                if (
                    !string.IsNullOrEmpty(group.NickName)
                    && options.Select(x => x.NickName).Contains(group.NickName)
                )
                {
                    var option = options.Where(x => x.NickName == group.NickName).FirstOrDefault();
                    // Set the group color
                    group.NickName = option.NickName;
                    group.Colour = option.Color;
                    group.Border = option.Border;
                }
            }
            //----------------------------------- Auto Recompute Component Options ------------------------------
            //ExpireSolution(true);
        }

        public List<GroupOptionDto> GetOptionData(List<GH_ColourSwatch> colourSwatches)
        {
            var options = new List<GroupOptionDto>();
            foreach (var swatch in colourSwatches)
            {
                var swatchgroup = GetGroupByComponetGuidId(swatch.ComponentGuid);
                options.Add(
                    new GroupOptionDto
                    {
                        NickName = swatch.NickName,
                        Color = swatch.SwatchColour,
                        Border = swatchgroup.Border,
                    }
                );
            }
            return options;
        }

        public GH_Group GetGroupByComponetGuidId(Guid componentGuid)
        {
            var doc = Grasshopper.Instances.ActiveCanvas.Document;
            return doc.Objects
                .OfType<GH_Group>()
                .Where(x => x.ObjectIDs.Contains(componentGuid))
                .FirstOrDefault();
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
