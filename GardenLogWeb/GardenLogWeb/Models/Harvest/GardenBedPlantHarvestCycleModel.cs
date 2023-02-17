using GardenLogWeb.Pages.GardenLayout.Components;

namespace GardenLogWeb.Models.Harvest
{
    public record GardenBedPlantHarvestCycleModel : GardenBedPlantHarvestCycleViewModel
    {
        public string ImageFileName { get; set; } = string.Empty;
        public string ImageLabel { get; set; } = string.Empty;

        public void SetLengthAndWidth(double bedLength, double bedWidth)
        {

            if (PlantsPerFoot > 1)
            {
                PatternWidth = 1;
                PatternLength = 1;
                // var plantsPerRow = (bedWidth / 12) * PlantsPerFoot;

                // Length = Math.Ceiling(NumberOfPlants / plantsPerRow);
                // Width = (bedWidth / 12);
            }
            else if (PlantsPerFoot == 1)
            {
                PatternWidth = 1;
                PatternLength = 1;
                // Length = Math.Ceiling(NumberOfPlants / (bedWidth / 12));
                // Width = Math.Round((bedWidth / 12), 0, MidpointRounding.ToZero);
            }
            else if (PlantsPerFoot < 1)
            {
                PatternWidth = Math.Ceiling(1 / PlantsPerFoot);
                PatternLength = PatternWidth;

                // Width = Math.Round((bedWidth / 12) / PatternWidth,0,MidpointRounding.ToZero);
                // Length =Math.Ceiling(NumberOfPlants / Width);

            }
            Length = PatternLength;
            Width = PatternWidth;
            Console.WriteLine($"PlantsPerFoot - {PlantsPerFoot} Length - {Length} Width - {Width}");
        }

        public double GetHeightInPixels()
        {
            return this.Length * GardenPlanSettings.TickFootHeight;
        }

        public double GetWidthInPixels()
        {
            return this.Width * GardenPlanSettings.TickFootWidth;
        }

        public double GetPatternHeightInPixels()
        {
            return this.PatternLength * GardenPlanSettings.TickFootHeight;
        }

        public double GetPatternWidthInPixels()
        {
            return this.PatternWidth * GardenPlanSettings.TickFootWidth;
        }

        public void MoveUp(int units)
        {
            Y -= units;
        }

        public void MoveDown(int units)
        {
            Y += units;
        }

        public void MoveLeft(int units)
        {
            X -= units;
        }

        public void MoveRight(int units)
        {
            X += units;
        }

        public void IncreaseLengthByPatternUnits(double patternUnits)
        {
            Length += patternUnits * PatternLength;
            if (Length <= 0) Length = 1;
            NumberOfPlants = Convert.ToInt32(PlantsPerFoot * Length * Width);
        }

        public void IncreaseWidthByPatternUnits(double patternUnits)
        {
            Width += patternUnits * PatternWidth;
            if (Width <= 0) Width = 1;
            NumberOfPlants = Convert.ToInt32(PlantsPerFoot * Length * Width);
        }


    }
}
