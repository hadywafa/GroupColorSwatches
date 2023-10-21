using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Special;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GroupColorSwatches
{
    public class GroupColorSwatchesComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GroupColorSwatchesComponent()
          : base("Group Color Swatches", "Auto Color Group",
            "Assign colors to current groups in gh document",
            "Hady Wafa", "Groups")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Groups NickNames", "Groups", "Group Names that need to change its color", GH_ParamAccess.list);
            pManager.AddColourParameter("Target Colors", "Colors", "Target Colors that you need to assign ti its corresponding Groups.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var groupNames = new List<string>();
            var targetColors = new List<System.Drawing.Color>();

            if (!DA.GetDataList(0, groupNames) || !DA.GetDataList(1, targetColors))
                return;

            var doc = Grasshopper.Instances.ActiveCanvas.Document;
            var currentGroups = doc.Objects.OfType<GH_Group>().ToList();

            foreach (var group in currentGroups)
            {
                if (!string.IsNullOrEmpty(group.NickName) && groupNames.Contains(group.NickName))
                {
                    int index = groupNames.IndexOf(group.NickName);
                    System.Drawing.Color color = targetColors[index];

                    // Set the group color
                    group.Colour = color;
                }
            }
            //----------------------------------------------------
            var colourSwatches = doc.Objects.OfType<GH_ColourSwatch>().ToList();
            var mainGroups = new List<GH_Group>();
            foreach (var group in currentGroups)
            {
                #region Test

                var xMenus = new ToolStripDropDown();
                xMenus.Items.Add("a7a");
                group.AppendMenuItems(xMenus);

                #endregion

                var groupComponents = group.Objects();
                if ( groupComponents.Count() == 1 && groupComponents.First() is GH_ColourSwatch)
                {
                    mainGroups.Add(group);
                }
            }
            var options = new List<GroupOptionDto>();
            foreach (var group in mainGroups)
            {
                var colourSwatchComponent = group.Objects().First() as GH_ColourSwatch;
                options.Add(new GroupOptionDto
                {
                    Name = colourSwatchComponent.NickName,
                    Color = colourSwatchComponent.SwatchColour,
                    Border = group.Border,
                });
            }
            //----------------------------------- Auto Recompute Component Options ------------------------------
            ExpireSolution(true);
        }
        public override GH_Exposure Exposure => GH_Exposure.primary;

        private bool previousInputTrigger = false;

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resources.icons8_color_palette.ToBitmap();

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("F3485AE6-E12F-4367-BD17-E1297BA16D26");
    }
}

public class GroupOptionDto
{
    public string Name { get; set; }
    public Color Color { get; set; }
    public GH_GroupBorder Border { get; set; }
}