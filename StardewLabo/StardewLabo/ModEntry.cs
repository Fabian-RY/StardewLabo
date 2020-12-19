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
using SObject = StardewValley.Object;

namespace StardewLabo
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod

    {
        // Constants

        private const string buildingType = "Laboratory";
        private readonly string BuildtPath = Path.Combine("Buildings", buildingType);
        private readonly string defaultTexturePath = Path.Combine("assets", "Laboratory.png");
        private bool LaboBuilt = false;
        private ITranslationHelper i18n;
        private Texture2D LaboOutdoorTexture;

        // Costes Laboratorio
        private int StoneCost = 100;
        private int WoodCost = 200;
        private int clayCost = 20;
        private int moneyCost = 10000;
        private int daysToBuild = 5;

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Display.MenuChanged += OnMenuChanged;
            i18n = helper.Translation;
        }


        /*********
        ** Private methods
        *********/
        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {       }

        private bool IsMagical(IClickableMenu menu)
        {
            return this.Helper.Reflection.GetField<bool>(menu, "magicalConstruction").GetValue();
            //return (bool)typeof(CarpenterMenu).GetField("magicalConstruction", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(carpenterMenu);
        }

        private bool HasBluePrint(IClickableMenu menu, string blueprintName)
        {
            return this.GetBluePrints(menu).Exists(bluePrint => bluePrint.name == blueprintName);
        }

        private List<BluePrint> GetBluePrints(IClickableMenu menu)
        {
            return this.Helper.Reflection.GetField<List<BluePrint>>(menu, "blueprints").GetValue();
            //return (List<BluePrint>)typeof(CarpenterMenu).GetField("blueprints", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).GetValue(carpenterMenu);
        }

        private void SetBluePrintField(BluePrint bluePrint, string field, object value)
        {
            this.Helper.Reflection.GetField<object>(bluePrint, field).SetValue(value);
            //typeof(BluePrint).GetField(field, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(bluePrint, value);
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e) {
            this.LaboOutdoorTexture = this.Helper.Content.Load<Texture2D>("assets/Labo.png");
        }

        /// <summary>The event called after an active menu is opened or closed.</summary>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // Si estamos hablando con Robin, añadimos la plantilla del Laboratorio
            if (e.NewMenu is CarpenterMenu carpenterMenu)
            {
                this.Monitor.Log("RobinBox!", LogLevel.Debug);
                if (!this.IsMagical(e.NewMenu) && !this.HasBluePrint(e.NewMenu, "Laboratory"))
                {
                    BluePrint LaboBluePrint = new BluePrint("Laboratory")
                    {
                        displayName = "Laboratorio",
                        description = "Permite almacenar máquinas biotecnológicas",
                        daysToConstruct = daysToBuild,//4
                        moneyRequired = moneyCost //40000
                    };
                    LaboBluePrint.itemsRequired.Clear();
                    LaboBluePrint.itemsRequired.Add(709, 200);//200
                    LaboBluePrint.itemsRequired.Add(330, 100);//100
                    LaboBluePrint.itemsRequired.Add(390, 100);//100
                    IList<BluePrint> blueprints = Helper.Reflection.GetField<List<BluePrint>>(carpenterMenu, "blueprints").GetValue();
                    blueprints.Add(LaboBluePrint);
                }
            }

        }
    }
}