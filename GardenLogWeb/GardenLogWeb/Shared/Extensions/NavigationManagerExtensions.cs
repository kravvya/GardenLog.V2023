﻿using Microsoft.AspNetCore.Components;

namespace GardenLogWeb.Shared.Extensions;

public static class NavigationManagerExtensions
{
    public static string GetPlantsUrl(this NavigationManager navigationManager)
    {
        return "plants";
    }

    public static string GetViewPlantUrl(this NavigationManager navigationManager, string plantId)
    {
        return $"viewplant/{plantId}";
    }

    public static string GetEditPlantUrl(this NavigationManager navigationManager, string plantId)
    {
        return $"editplant/{plantId}";
    }

    public static string GetCreatePlantUrl(this NavigationManager navigationManager)
    {
        return "addplant";
    }

    public static string GetViewPlantVarietyUrl(this NavigationManager navigationManager, string plantId, string plantVarietyId)
    {
        return $"plantvariety/{plantId}/variety/{plantVarietyId}";
    }

    public static string GetCreatePlantVarietyUrl(this NavigationManager navigationManager, string plantId)
    {
        return $"addplantvariety/{plantId}";
    }

    public static string GetEditPlantVarietyUrl(this NavigationManager navigationManager, string plantId, string plantVarietyId)
    {
        return $"editplantvariety/{plantId}/variety/{plantVarietyId}";
    }

    public static string GetViewPlantGrowInstructionUrl(this NavigationManager navigationManager, string plantId, string growInstructionId)
    {
        return $"plantgrow /{ plantId}/grow/{ growInstructionId}";
    }

    public static string GetCreatePlantGrowInstructionUrl(this NavigationManager navigationManager, string plantId)
    {
        return $"addplantgrow/{plantId}";
    }

    public static string GetEditPlantGrowInstructionUrl(this NavigationManager navigationManager, string plantId, string growInstructionId)
    {
        return $"editplantgrow/{plantId}/grow/{growInstructionId}";
    }

    public static string GetGardenPlansUrl(this NavigationManager navigationManager)
    {
        return $"garden_plan";
    }

    public static string GetGardenPlanUrl(this NavigationManager navigationManager, string harvestId)
    {
        return $"garden_plan/{harvestId}";
    }

    public static string GetGardenPlanImagesUrl(this NavigationManager navigationManager, string harvestId)
    {
        return $"images/garden_plan/{harvestId}";
    }

    public static string GetGardenPlanWorkLogsUrl(this NavigationManager navigationManager, string harvestId)
    {
        return $"worklogs/garden_plan/{harvestId}";
    }

    public static string GetGardenPlanAddPlantUrl(this NavigationManager navigationManager, string harvestId)
    {
        return $"/addplant/garden_plan/{harvestId}";
    }

    public static string GetGardenPlanEditPlantUrl(this NavigationManager navigationManager, string harvestId, string plantHarvestId)
    {
        return $"/editplant/garden_plan/{harvestId}/plant/{plantHarvestId}";
    }

    public static void NavigateToPlants(this NavigationManager navigationManager)
    {
        navigationManager.NavigateTo(navigationManager.GetPlantsUrl());
    }

    public static void NavigateToCreatePlant(this NavigationManager navigationManager)
    {
        navigationManager.NavigateTo(navigationManager.GetCreatePlantUrl());
    }

    public static void NavigateToViewPlant(this NavigationManager navigationManager, string plantId)
    {
        navigationManager.NavigateTo(navigationManager.GetViewPlantUrl(plantId));
    }

    public static void NavigateToEditPlant(this NavigationManager navigationManager, string plantId)
    {
        navigationManager.NavigateTo(navigationManager.GetEditPlantUrl(plantId));
    }

    public static void NavigateToPlantVariety(this NavigationManager navigationManager, string plantId, string plantVarietyId)
    {
        navigationManager.NavigateTo(navigationManager.GetViewPlantVarietyUrl(plantId, plantVarietyId));
    }

    public static void NavigateToCreatePlantVariety(this NavigationManager navigationManager, string plantId)
    {
        navigationManager.NavigateTo(navigationManager.GetCreatePlantVarietyUrl(plantId));
    }

    public static void NavigateToEditPlantVariety(this NavigationManager navigationManager, string plantId, string plantVarietyId)
    {
        navigationManager.NavigateTo(navigationManager.GetEditPlantVarietyUrl(plantId, plantVarietyId));
    }

    public static void NavigateToPlantGrowInstruction(this NavigationManager navigationManager, string plantId, string growInstructionId)
    {
        navigationManager.NavigateTo(navigationManager.GetViewPlantGrowInstructionUrl(plantId, growInstructionId));
    }

    public static void NavigateToCreatePlantGrowInstruction(this NavigationManager navigationManager, string plantId)
    {
        navigationManager.NavigateTo(navigationManager.GetCreatePlantGrowInstructionUrl(plantId));
    }

    public static void NavigateToEditPlantGrowInstruction(this NavigationManager navigationManager, string plantId, string growInstructionId)
    {
        navigationManager.NavigateTo(navigationManager.GetEditPlantGrowInstructionUrl(plantId, growInstructionId));
    }

    public static void NavigateToGardenPlans(this NavigationManager navigationManager)
    {
        navigationManager.NavigateTo(navigationManager.GetGardenPlansUrl());
    }

    public static void NavigateToGardenPlan(this NavigationManager navigationManager, string harvestId)
    {
        navigationManager.NavigateTo(navigationManager.GetGardenPlanUrl(harvestId));
    }


    public static void NavigateToGardenPlanImages(this NavigationManager navigationManager, string harvestId)
    {
        navigationManager.NavigateTo(navigationManager.GetGardenPlanImagesUrl(harvestId));
    }

    public static void NavigateToGardenPlanWorkLogs(this NavigationManager navigationManager, string harvestId)
    {
        navigationManager.NavigateTo(navigationManager.GetGardenPlanWorkLogsUrl(harvestId));
    }

    public static void NavigateToGardenPlanAddPlant(this NavigationManager navigationManager, string harvestId)
    {
        navigationManager.NavigateTo(navigationManager.GetGardenPlanAddPlantUrl(harvestId));
    }

    public static void NavigateToGardenPlanEditPlant(this NavigationManager navigationManager, string harvestId, string plantHarvestId)
    {
        navigationManager.NavigateTo(navigationManager.GetGardenPlanEditPlantUrl(harvestId, plantHarvestId));
    }
}