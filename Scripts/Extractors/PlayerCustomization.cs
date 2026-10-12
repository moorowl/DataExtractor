using Newtonsoft.Json.Linq;
using System.Linq;
using DataExtractor.Utilities.Extensions;
using UnityEngine.AddressableAssets;

namespace DataExtractor.Extractors {
	public class PlayerCustomization : Extractor {
		public override string Name => "playerCustomization";
		public override JToken Extract() {
			var customizationTable = Manager.saves.playerCustomizationTable.Get();

			var hairSourceColors = customizationTable.hairSourceColors.Get();
			var hairShadeSourceColors = customizationTable.hairShadeSourceColors.Get();
			
			var data = new {
				Helm = new {
					Skins = customizationTable.helmSkinCollection.Get().OrderBy(helmSkin => helmSkin.GetRuntimeID()).Select(helmSkin => new {
						Guid = helmSkin.address.ToString(),
						InternalName = helmSkin.name,
						Texture = GetTextureName(helmSkin.helmTexture),
						EmissiveTexture = GetTextureName(helmSkin.emissiveHelmTexture),
						HairType = helmSkin.hairType,
						Offset = new {
							X = helmSkin.pixelOffset.x,
							Y = helmSkin.pixelOffset.y
						}
					})
				},
				BreastArmor = new {
					Skins = customizationTable.breastArmorSkinCollection.Get().OrderBy(helmSkin => helmSkin.GetRuntimeID()).Select(breastArmorSkin => new {
						Guid = breastArmorSkin.address.ToString(),
						InternalName = breastArmorSkin.name,
						Texture = GetTextureName(breastArmorSkin.breastTexture),
						EmissiveTexture = GetTextureName(breastArmorSkin.emissiveBreastTexture),
						ShirtVisibility = breastArmorSkin.shirtVisibility
					})
				},
				PantsArmor = new {
					Skins = customizationTable.pantsArmorSkinCollection.Get().OrderBy(helmSkin => helmSkin.GetRuntimeID()).Select(pantsArmorSkin => new {
						Guid = pantsArmorSkin.address.ToString(),
						InternalName = pantsArmorSkin.name,
						Texture = GetTextureName(pantsArmorSkin.pantsTexture),
						EmissiveTexture = GetTextureName(pantsArmorSkin.emissivePantsTexture),
						PantsVisibility = pantsArmorSkin.pantsVisibility
					})
				},
				Hair = new {
					Skins = customizationTable.hairSkinCollection.Get().OrderBy(hairSkin => hairSkin.GetRuntimeID()).Select(hairSkin => new {
						Guid = hairSkin.address.ToString(),
						InternalName = hairSkin.name,
						Texture = GetTextureName(hairSkin.hairTexture),
						HelmTexture = GetTextureName(hairSkin.helmHairTexture),
						ShadeTexture = GetTextureName(hairSkin.hairShadeTexture)
					}),
					SourceColors = new {
						Guid = hairSourceColors.address.ToString(),
						InternalName = hairSourceColors.name,
						Colors = hairSourceColors.colors.Select(color => color.ToHex())
					},
					ReplacementColors = customizationTable.hairReplacementColors.Get().OrderBy(replacementColors => replacementColors.GetRuntimeID()).Select(replacementColors => new {
						Guid = replacementColors.address.ToString(),
						InternalName = replacementColors.name,
						Colors = replacementColors.colors.Select(color => color.ToHex())
					}),
					ShadeSourceColors = new {
						Guid = hairShadeSourceColors.address.ToString(),
						InternalName = hairShadeSourceColors.name,
						Colors = hairShadeSourceColors.colors.Select(color => color.ToHex())
					},
					ShadeReplacementColors = customizationTable.hairShadeReplacementColors.Get().OrderBy(replacementColors => replacementColors.GetRuntimeID()).Select(replacementColors => new {
						Guid = replacementColors.address.ToString(),
						InternalName = replacementColors.name,
						Colors = replacementColors.colors.Select(color => color.ToHex())
					}),
				}
			};

			return Serialize(data);
		}

		private static string GetTextureName(AssetReferenceTexture2D reference) {
			if (reference == null || !reference.RuntimeKeyIsValid())
				return null;
			
			reference.LoadAssetAsync().WaitForCompletion();
			return reference.Asset.name;
		}

		/*public override JToken Extract() {
			var customizationTable = ((CharacterCustomizationMenu) Manager.menu.characterCustomizationMenu).roleSelection.customizationTable;

			var data = new {
				Helm = new {
					Skins = customizationTable.helmSkins.Select((skin, index) => new {
						Id = customizationTable.GetIdFromIndex(PlayerCustomizationTable.CustomizableBodyPartType.HELM, index),
						Texture = skin.helmTexture?.name,
						EmissiveTexture = skin.emissiveHelmTexture?.name,
						HairType = skin.hairType,
						Offset = new {
							X = skin.pixelOffset.x,
							Y = skin.pixelOffset.y
						}
					})
				},
				BreastArmor = new {
					Skins = customizationTable.breastArmorSkins.Select((skin, index) => new {
						Id = customizationTable.GetIdFromIndex(PlayerCustomizationTable.CustomizableBodyPartType.BREAST_ARMOR, index),
						Texture = skin.breastTexture?.name,
						EmissiveTexture = skin.emissiveBreastTexture?.name,
						ShirtVisibility = skin.shirtVisibility
					})
				},
				PantsArmor = new {
					Skins = customizationTable.pantsArmorSkins.Select((skin, index) => new {
						Id = customizationTable.GetIdFromIndex(PlayerCustomizationTable.CustomizableBodyPartType.PANTS_ARMOR, index),
						Texture = skin.pantsTexture?.name,
						EmissiveTexture = skin.emissivePantsTexture?.name,
						PantsVisibility = skin.pantsVisibility
					})
				}
			};

			return Serialize(data);
		}*/
	}
}