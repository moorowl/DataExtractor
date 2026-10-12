using Newtonsoft.Json.Linq;
using System.Linq;
using DataExtractor.Utilities;

namespace DataExtractor.Extractors {
	public class ContentBundles : Extractor {
		public override string Name => "contentBundles";

		public override JToken Extract() {
			var contentBundles = ScriptableData.GetDataBlocks<ContentBundleDataBlock>();

			var data = contentBundles.OrderBy(contentBundle => contentBundle.name)
				.Select(contentBundle => new {
					Guid = contentBundle.address.guid,
					InternalName = contentBundle.name,
					DisplayName = Utils.GetTranslations($"ContentBundles/{contentBundle.name}"),
					Description = Utils.GetTranslations($"ContentBundles/{contentBundle.name}Desc"),
					CreatedForVersion = contentBundle.createdForVersion,
					CanBeActivatedByPlayer = contentBundle.canBeActivatedByPlayer,
					DisplayOrder = (int?) (contentBundle.canBeActivatedByPlayer ? contentBundle.displayOrder : null),
					AutomaticallyAddedToNewWorlds = contentBundle.automaticallyAddedToNewWorlds,
					EnabledIfSeedContainsString = contentBundle.enabledIfSeedContainsString.hasValue ? contentBundle.enabledIfSeedContainsString.value : null,
					Dependencies = contentBundle.dependencies.Select(contentBundleRef => contentBundleRef.Get().name).ToList()
				});

			return Serialize(data);
		}
	}
}