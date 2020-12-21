using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile;
using xTile.Tiles;
using SObject = StardewValley.Object;

namespace StardewLabo
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetLoader

    {
        // Constants

        private const string buildingType = "Laboratory";
        private readonly string BuildtPath = Path.Combine("Buildings", buildingType);
        private readonly string defaultPath = Path.Combine("assets", "Laboratory.png");
        private ITranslationHelper i18n;
        private Map LaboInside;
        private Texture2D LaboOutdoor;

        // Costes Laboratorio
        private int StoneCost = 100;
        private int WoodCost = 200;
        private int clayCost = 20;
        private int moneyCost = 10000;
        private int daysToBuild = 0;

        private DataSaved data;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.GameLoop.DayEnding += OnDayEnding;
            helper.Events.GameLoop.Saving += this.OnSaving;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.Saved += this.OnSaved;
            i18n = helper.Translation;
            //LaboInside = helper.Content.Load<Map>("assets/LaboInside.tbin");
            this.LaboOutdoor = this.Helper.Content.Load<Texture2D>("assets/Labo.png");
            this.LaboInside = this.Helper.Content.Load<Map>("assets\\Winery.tbin");
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e) {
            Monitor.Log("OnDayStarted", LogLevel.Debug);
        }

        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            Monitor.Log("Day Ended", LogLevel.Debug);
        }
        
        // Changes the Shed saved in the data into the Laboratory
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            this.data = this.Helper.Data.ReadSaveData<DataSaved>("LaboData");
            Monitor.Log("Mod Data loaded", LogLevel.Debug);
            //Monitor.Log(this.data.ToString(), LogLevel.Debug);
            if (this.data == null)
            {
                this.data = new DataSaved();
            }
            foreach (Building b in Game1.getFarm().buildings)
            {
                /// Cambiamos el tipo de edificio si encontramos una cabaña en el lugar donde está el laboratorio
                if (b.buildingType.Value.Equals("Shed") && this.data.LaboCoordinatesX == b.tileX.Value && this.data.LaboCoordinatesY == b.tileY.Value )
                {
                    b.buildingType.Value = "Laboratory";
                    b.indoors.Value.mapPath.Value = "Maps\\Laboratory";
                    b.indoors.Value.updateMap();
                }
            }
        }
        /// Changes The laboratory into a shed to be saved in file
        private void OnSaving(object sender, SavingEventArgs e)
        {
            Monitor.Log("Saving", LogLevel.Debug);
            foreach (Building b in Game1.getFarm().buildings)
            {
                Monitor.Log(b.buildingType.Value, LogLevel.Debug);
                if (b.buildingType.Value.Equals("Laboratory"))
                {
                    b.buildingType.Value = "Shed";
                    b.indoors.Value.mapPath.Value = "Maps\\Shed";
                    b.indoors.Value.updateMap();
                    Monitor.Log("Entering Laboratory", LogLevel.Debug);
                    Monitor.Log("TileX: " + b.tileX.Value.ToString(), LogLevel.Debug);
                    Monitor.Log("TileY: " + b.tileY.Value.ToString(), LogLevel.Debug);
                    Monitor.Log("LaboX: " + this.data.LaboCoordinatesX.ToString(), LogLevel.Debug);
                    Monitor.Log("Laboy: " + this.data.LaboCoordinatesY.ToString(), LogLevel.Debug);
                    this.data.LaboCoordinatesX = b.tileX.Value;
                    this.data.LaboCoordinatesY = b.tileY.Value;
                    this.data.LaboBuilded = true;
                    this.Monitor.Log("Labo X:"+b.tileX.Value.ToString()+", Y:"+b.tileY.Value, LogLevel.Debug);

                }
            }
            this.Helper.Data.WriteSaveData("LaboData", this.data);
        }

        /// Changes the Shed saved in the data into the Laboratory, just like in SaveLoaded
        /// TO DO: make a function that does this so it's the same code for both
        private void OnSaved(object sender, SavedEventArgs e)
        {
            foreach (Building b in Game1.getFarm().buildings)
            {
                Monitor.Log(b.buildingType.Value, LogLevel.Debug);
                if (b.buildingType.Value.Equals("Shed") && this.data.LaboCoordinatesX == b.tileX.Value && this.data.LaboCoordinatesY == b.tileY.Value)
                {
                    b.buildingType.Value = "Laboratory";
                    b.indoors.Value.mapPath.Value = "Maps\\Laboratory";
                    b.indoors.Value.updateMap();
                }
            }
        }

        /// <summary>The event called after an active menu is opened or closed.</summary>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // Si estamos hablando con Robin, añadimos la plantilla del Laboratorio
            if (e.NewMenu is CarpenterMenu carpenterMenu)
            {
                this.Monitor.Log("RobinBox!", LogLevel.Debug);
                this.Monitor.Log("Adding Bluprints");
                BluePrint LaboBluePrint = new BluePrints().LaboBlueprint(this.Helper);
                IList<BluePrint> blueprints = this.Helper.Reflection.GetField<List<BluePrint>>(carpenterMenu, "blueprints").GetValue();
                blueprints.Add(LaboBluePrint);
            }
        }


        public bool CanLoad<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Buildings/Laboratory") || asset.AssetNameEquals("Maps\\Laboratory");
        }

        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Buildings/Laboratory"))
                return (T)(object)this.LaboOutdoor;

            else if (asset.AssetNameEquals("Maps\\Laboratory"))
                return (T)(object)this.LaboInside;

            return (T)(object)null;
        }
    }
}