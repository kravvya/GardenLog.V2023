﻿using ImageCatalog.Contract.ViewModels;
using PlantCatalog.Contract.Validators;

namespace GardenLogWeb.Models.Plants;

public record PlantModel : PlantViewModel
{
    public List<ImageViewModel> Images { get; set; }
    public string ImageFileName { get; set; }
    public string ImageLabel { get; set; }
}