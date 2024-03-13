using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GoodHub.Core.Runtime;
using UnityEngine;

public class OverlayGraphicsController : SceneSingleton<OverlayGraphicsController>
{

    [SerializeField] private GameObject _stackedCharactersOverlayPrefab;
    [SerializeField] private RectTransform _stackedCharactersOverlayContainer;


    private List<StackedCharactersOverlay> _stackedCharactersOverlays = new List<StackedCharactersOverlay>();


    public void RefreshStackedCharacterOverlays(List<CharacterEntity> characterEntities, CharacterEntity activeCharacterEntity)
    {
        Dictionary<Vector3Int, List<CharacterEntity>> charactersByPosition = new Dictionary<Vector3Int, List<CharacterEntity>>();

        foreach (CharacterEntity characterEntity in characterEntities)
        {
            if (charactersByPosition.TryGetValue(characterEntity.Position, out List<CharacterEntity> entitiesList))
            {
                entitiesList.Add(characterEntity);
            }
            else
            {
                charactersByPosition.Add(characterEntity.Position, new List<CharacterEntity> { characterEntity });
            }
        }

        List<Vector3Int> overlayPositions = new List<Vector3Int>();

        foreach ((Vector3Int position, List<CharacterEntity> characters) in charactersByPosition)
        {
            if (characters.Count == 1)
                continue;

            overlayPositions.Add(position);

            StackedCharactersOverlay existingOverlay = _stackedCharactersOverlays.Find(overlay => overlay.TrackedTile == position);

            if (existingOverlay != null)
            {
                existingOverlay.Initialise(characters, position);
                continue;
            }

            StackedCharactersOverlay stackedCharactersOverlay = Instantiate(_stackedCharactersOverlayPrefab, _stackedCharactersOverlayContainer).GetComponent<StackedCharactersOverlay>();
            stackedCharactersOverlay.Initialise(characters, position);

            _stackedCharactersOverlays.Add(stackedCharactersOverlay);
        }

        // Remove any overlays no longer needed
        for (int index = _stackedCharactersOverlays.Count - 1; index >= 0; index--)
        {
            if (overlayPositions.Contains(_stackedCharactersOverlays[index].TrackedTile))
                continue;

            Destroy(_stackedCharactersOverlays[index].gameObject);
            _stackedCharactersOverlays.RemoveAt(index);
        }
    }

}