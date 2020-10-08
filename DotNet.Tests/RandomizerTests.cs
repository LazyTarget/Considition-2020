using System;
using System.Diagnostics;
using System.Text;
using DotNet.Logging;
using DotNet.Strategy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Workers = 4, Scope = ExecutionScope.MethodLevel)]

namespace DotNet.Tests
{
	[TestClass]
	public abstract class RandomizerTests
	{
		private string ApiKey;
		protected virtual string Map { get; set; }

		private GameRunner _runner;
		private StringBuilder _output;


		protected RandomizerTests()
		{
			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.AddEnvironmentVariables("CONSIDITION_")
				.AddUserSecrets<RandomizerTests>(true)
				.Build();
			ApiKey = configuration.GetValue<string>("ApiKey");
			if (string.IsNullOrWhiteSpace(ApiKey))
				throw new ArgumentNullException(nameof(ApiKey));
		}

		[TestInitialize]
		public void Initialize()
		{
			TestInitialize();
		}

		protected virtual void TestInitialize() { }

		[TestCleanup]
		public void Cleanup()
		{
			_runner?.EndGame();

			var fullLog = _output.ToString();
			Debug.WriteLine(fullLog);
		}

		protected virtual GameRunner GetRunner(string map = null)
		{
			if (_runner != null)
				return _runner;

			map ??= Map;
			if (string.IsNullOrWhiteSpace(map))
			{
				Assert.Inconclusive("Missing Map-argument!");
			}
			try
			{
				_output = new StringBuilder();
				var loggerFactory = LoggerFactory.Create(c => c
					.AddProvider(new InMemoryLoggerProvider(_output)));

				var runner = GameRunner.New(ApiKey, map, loggerFactory);
				_runner = runner;
				return runner;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		[TestMethod]
		public void DefaultStrategy()
		{
			var runner = GetRunner();
			var score = runner.Run();
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_Apartments()
		{
			var buildingName = "Apartments";

			var strategy = TurnStrategyBase
				.Create<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c => c.BuildingName = buildingName)
				.Append<BuyUpgradeTurnStrategy>()
				.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
				.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
				.Append<AdjustBuildingTemperaturesTurnStrategy>()
				.Append<BuildBuildingOnTurnZeroTurnStrategy>(c => c.BuildingName = "Cabin");

			var score = GetRunner().Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_ModernApartments()
		{
			var buildingName = "ModernApartments";

			TurnStrategyBase strategy = null;
			strategy = new BuildBuildingWhenCloseToPopMaxTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = "Cabin",
			};

			var score = GetRunner().Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_Cabin()
		{
			var buildingName = "Cabin";

			TurnStrategyBase strategy = null;
			strategy = new BuildBuildingWhenCloseToPopMaxTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = "Cabin",
			};

			var score = GetRunner().Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_EnvironmentalHouse()
		{
			var buildingName = "EnvironmentalHouse";

			TurnStrategyBase strategy = null;
			strategy = new BuildBuildingWhenCloseToPopMaxTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = "Cabin",
			};

			var score = GetRunner().Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_HighRise()
		{
			var buildingName = "HighRise";

			TurnStrategyBase strategy = null;
			strategy = new BuildBuildingWhenCloseToPopMaxTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = "Cabin",
			};

			var score = GetRunner().Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_LuxuryResidence()
		{
			var buildingName = "LuxuryResidence";

			TurnStrategyBase strategy = null;
			strategy = new BuildBuildingWhenCloseToPopMaxTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = "Cabin",
			};

			var score = GetRunner().Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void WithoutStartBuildOnTurnZero()
		{
			var strategy = TurnStrategyBase
				.Create<BuildBuildingWhenCloseToPopMaxTurnStrategy>()
				.Append<BuyUpgradeTurnStrategy>()
				.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
				.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
				.Append<AdjustBuildingTemperaturesTurnStrategy>();
				//.Append<BuildBuildingOnTurnZeroTurnStrategy>(c => c.BuildingName = "Cabin");

			var score = GetRunner().Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}


		[TestClass]
		public class training1Map : RandomizerTests
		{
			protected override string Map { get; set; } = "training1";
		}

		[TestClass]
		public class training2Map : RandomizerTests
		{
			protected override string Map { get; set; } = "training2";
		}

		[TestClass]
		public abstract class Strategy : RandomizerTests
		{
			protected abstract TurnStrategyBase GetStrategy();

			[TestMethod]
			public virtual void training1()
			{
				Map = "training1";
				var strategy = GetStrategy();

				var runner = GetRunner();
				var score = runner.Run(strategy);
				Assert.IsTrue(score.FinalScore > 0);
			}

			[TestMethod]
			public virtual void training2()
			{
				Map = "training2";
				var strategy = GetStrategy();

				var runner = GetRunner();
				var score = runner.Run(strategy);
				Assert.IsTrue(score.FinalScore > 0);
			}
			
			[TestClass]
			public class Default : Strategy
			{
				protected override TurnStrategyBase GetStrategy()
				{
					return null;
				}
			}

			[TestClass]
			public class Custom : Strategy
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = TurnStrategyBase
						.Create<BuildBuildingWhenCloseToPopMaxTurnStrategy>()
						.Append<BuyUpgradeTurnStrategy>()
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>();
					return strategy;
				}
			}
		}
	}
}
