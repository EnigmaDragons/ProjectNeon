using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class CardScreenshotExporter : MonoBehaviour
{
    [SerializeField] private string baseExportPathDir;
    [SerializeField] private Library heroes;
    [SerializeField] private AllCards cards;
    [SerializeField] private CardPresenter cardPresenter;
    [SerializeField] private Vector2 captureSize;
    [SerializeField] private bool takeAll = false;
    [SerializeField] private Color transparentColor = Color.black;

    private void Start() => this.SafeCoroutineOrNothing(Go());

    private IEnumerator Go()
    {
        yield return new WaitForEndOfFrame();
        foreach (var h in heroes.UnlockedHeroes)
        {
            var member = GetHeroCards(h, out var heroCards);

            foreach (var c in heroCards)
            {
                var card = new Card(-1, member, c, h.Tint, h.Bust);
                var err = false;
                try
                {
                    cardPresenter.Set(card);
                }
                catch (Exception e)
                {
                    err = true;
                    Log.Warn($"Unable to Render {card.Name}");
                    Log.Error(e);
                }

                if (!err)
                {
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();
                    ExportCard(h, c);
                    
                    if (!takeAll)
                        yield break;
                }
            }
        }
        Log.Info("Finished Export");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void ExportCard(BaseHero h, CardTypeData c)
    {
        var savePath = Path.Combine(baseExportPathDir, h.NameTerm().ToEnglish() + "-" + c.Name.Replace(" ", "").Replace("\"", "") + ".png");
        var tex = ScreenCapture.CaptureScreenshotAsTexture();

        var newTexture = new Texture2D(tex.width, tex.height, TextureFormat.ARGB32, false);
        var pixels = tex.GetPixels();
        for (var i = 0; i < pixels.Length; i++)
        {
            var p = pixels[i];
            if (p.r == transparentColor.r && p.g == transparentColor.g && p.b == transparentColor.b)
                p = Color.clear;
        }

        newTexture.SetPixels(pixels);
        var pngShot = ImageConversion.EncodeToPNG(newTexture);
        File.WriteAllBytes(savePath, pngShot);
    }

    private Member GetHeroCards(BaseHero h, out IEnumerable<CardTypeData> heroCards)
    {
        var member = h.AsMemberForLibrary();
        var archKeys = h.ArchetypeKeys();
        var excludedCards = h.ExcludedCards;
        heroCards = cards.Cards
            .Where(c => !c.IsWip)
            .Where(c => archKeys.Contains(c.GetArchetypeKey()) || c.Archetypes.None())
            .Where(c => c.Rarity != Rarity.Basic)
            .Where(c => !excludedCards.Contains(c))
            .Where(c => c.Archetypes.Any())
            .OrderBy(c => c.Archetypes.None() ? 99 : c.Archetypes.Count)
            .ThenBy(c => c.GetArchetypeKey())
            .ThenBy(c => c.Rarity)
            .ThenBy(c => c.Cost.BaseAmount)
            .ThenBy(c => c.Name)
            .Concat(h.BasicCard)
            .Concat(h.ParagonCards);
        return member;
    }
}
