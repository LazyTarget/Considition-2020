﻿using System;
using System.Diagnostics;
using System.Linq;
using DotNet.Interfaces;
using DotNet.models;
using Microsoft.Extensions.Logging;

namespace DotNet.Strategy
{
	public class BuildBuildingWhenCloseToPopMaxTurnStrategy : TurnStrategyBase
	{
		public BuildBuildingWhenCloseToPopMaxTurnStrategy() : base()
		{
		}

		public BuildBuildingWhenCloseToPopMaxTurnStrategy(TurnStrategyBase parent) : base(parent)
		{
		}

		public string BuildingName { get; set; }

		public double PopulationPercentageThreshold { get; set; } = 0.805;

		public int MaxNumberOfResidences { get; set; } = 10;

		protected override bool TryExecuteTurn(Randomizer randomizer, IGameLayer gameLayer, GameState state)
		{
			var buildings = state.GetBuiltBuildings().OfType<BuiltResidenceBuilding>().ToArray();
			if (buildings.Length >= MaxNumberOfResidences)
			{
				// Don't build any more buildings
				return false;
			}

			var completedResidences = state.GetCompletedBuildings().OfType<BuiltResidenceBuilding>().ToArray();

			// Only build when has nothing else to do
			var currentPop = completedResidences.Sum(x => x.CurrentPop);

			var currentPopMax = completedResidences
				.Join(state.AvailableResidenceBuildings, ok => ok.BuildingName, ik => ik.BuildingName,
					(rb, bp) => new {bp, rb})
				.Sum(x => x.bp.MaxPop);

			var pendingPopMaxIncrease = state.GetBuildingsUnderConstruction().OfType<BuiltResidenceBuilding>()
				.Join(state.AvailableResidenceBuildings, ok => ok.BuildingName, ik => ik.BuildingName,
					(rb, bp) => new {bp, rb})
				.Sum(x => x.bp.MaxPop);

			var currentPopPercentage = currentPopMax > 0 ? currentPop / (double)currentPopMax : 0;


			Logger.LogDebug($"Pop {currentPop}/{currentPopMax} = {currentPopPercentage:P2}		(+ {pendingPopMaxIncrease})");

			if (currentPopPercentage > PopulationPercentageThreshold || 
			    buildings.Length < 1)
			{
				var position = randomizer.GetRandomBuildablePosition();
				if (position == null)
				{
					Logger.LogWarning("No valid positions to build building");
					return false;
				}

				BlueprintResidenceBuilding building;
				var buildingName = BuildingName;
				if (!string.IsNullOrWhiteSpace(buildingName))
				{
					building = state.AvailableResidenceBuildings.Find(x => x.BuildingName == buildingName);
				}
				else
				{
					var affordableBuildings = state.AvailableResidenceBuildings
						.Where(x => x.Cost <= state.Funds)
						.ToArray();
					building = affordableBuildings.ElementAtOrDefault(randomizer.Random.Next(0, affordableBuildings.Length));
				}

				if (building == null)
				{
					// No valid building
					return false;
				}

				if (building.Cost > state.Funds)
				{
					// Cannot afford it
					Logger.LogWarning("Wanted to build building, but cannot afford it");
					return false;
				}

				gameLayer.StartBuild(position, building.BuildingName, state.GameId);
				return true;
			}
			return false;
		}
	}
}