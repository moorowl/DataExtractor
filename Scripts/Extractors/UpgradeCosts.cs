using Newtonsoft.Json.Linq;
using System.Linq;

namespace DataExtractor.Extractors {
	public class UpgradeCosts : Extractor {
		public override string Name => "upgradeCosts";

		public override JToken Extract() {
			var upgradeCostsAuthoring = Manager.ecs.serverAuthoringPrefab.GetComponentInChildren<UpgradeCostsTableAuthoring>();

			var data = UpgradeCostsTable.GetTable().upgradeCosts.Select((costs, index) => {
				return new {
					Level = index,
					Materials = costs.upgradeCost.Select(upgradeCost => new {
						Id = upgradeCost.item,
						Amount = upgradeCost.amount
					})
				};
			});

			return Serialize(data);
		}
	}
}