using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int _cargoMaxLenght = 3;
    [SerializeField] private int _cargoMaxWidth = 3;
    [SerializeField] private int _cargoHeight = 3;
    [SerializeField] private WareCollection _allowedCollection;

    [Header("References")]
    [SerializeField] private CargoSlot[] _slots;
    [SerializeField] private LayerMask _wareLayerMask;
    [SerializeField] private ParticleSystem _cargoCompletedParticles;
    [SerializeField] private ParticleSystem _lineWinParticles; //prefab

    public WareCollection AllowedCollection => _allowedCollection;
    public float FillPercentage => _fillPercentage;
    public int SlotCount => _slotCount;
    public int OccupiedSlotCount => _occupiedSlotCount;
    public int EmptySlotCount => _emptySlotCount;
    public int CargoSize => _cargoHeight;
    public ParticleSystem CargoCompletedParticles => _cargoCompletedParticles;

    private List<ParticleSystem> _lineListParticleSystems;
    private List<Ware> _placedWare;
    private Dictionary<Ware.WareTypes, int> _typesCounter;
    private int _slotCount;
    private float _fillPercentage;
    private int _occupiedSlotCount;
    private int _emptySlotCount;
    private List<int> _fullLines;

    private void Start()
    {
        _typesCounter = new Dictionary<Ware.WareTypes, int>();
        _fullLines = new List<int>();
        _placedWare = new List<Ware>();
        foreach (CargoSlot cargoSlot in _slots)
        {
            cargoSlot.Initialize(this);
        }

        _slotCount = _slots.Length * _cargoHeight;
        _emptySlotCount = _slotCount;
        _occupiedSlotCount = 0;

        if (_cargoCompletedParticles == null)
        {
            Debug.LogError("Cargo completed particles not set in inspector!");
        }

        if (_lineWinParticles == null)
        {
            Debug.LogError("Line win particles not set in inspector!");
        }
        else
        {
            _lineListParticleSystems = new List<ParticleSystem>(_cargoHeight);
            for (int i = 0; i < _cargoHeight; i++)
            {
                ParticleSystem lineWinParticles = Instantiate(_lineWinParticles, transform);
                lineWinParticles.transform.localPosition = new Vector3(0, 0.5f + i * 1, 0);
                lineWinParticles.transform.localScale = new Vector3(_cargoMaxWidth * lineWinParticles.transform.localScale.x, lineWinParticles.transform.localScale.y, _cargoMaxLenght * lineWinParticles.transform.localScale.z);
                _lineListParticleSystems.Add(lineWinParticles);
            }
        }
    }

    public void AddWare(Ware ware)
    {
        _placedWare.Add(ware);
        AddWareTypes(ware.GetWareType());
        UpdateCargoContent();
        CheckForFullLines();

        // TODO: add ware interaction trigger here
        AudioManager.Instance.PlaySoundEffect(SoundEffectType.POSE_BLOCK);
    }

    public void RemoveWare(Ware ware)
    {
        _placedWare.Remove(ware);
        RemoveWareTypes(ware.GetWareType());
        UpdateCargoContent();

        AudioManager.Instance.PlaySoundEffect(SoundEffectType.CANCEL_BLOCK);
    }

    public void UpdateCargoContent()
    {
        _occupiedSlotCount = 0;

        foreach (Ware ware in _placedWare)
        {
            _occupiedSlotCount += ware.Size;
        }

        _emptySlotCount = _slotCount - _occupiedSlotCount;

        if (_occupiedSlotCount > 0)
        {
            _fillPercentage = (float)_occupiedSlotCount / _slotCount * 100f;
        }
    }

    void AddWareTypes(Ware.WareTypes wareType)
    {
        if (!_typesCounter.ContainsKey(wareType))
        {
            _typesCounter[wareType] = 0;
        }
        _typesCounter[wareType]++;
    }

    void RemoveWareTypes(Ware.WareTypes wareType)
    {
        if (!_typesCounter.ContainsKey(wareType))
        {
            _typesCounter[wareType] = 0;
        }
        _typesCounter[wareType]--;
    }

    public void ActivateCargo()
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            Collider[] colliders = _slots[i].GetComponents<Collider>();
            for (int j = 0; j < colliders.Length; j++)
            {
                Collider collider = colliders[j];
                collider.enabled = true;
            }
        }
    }

    private void CheckForFullLines()
    {
        for (int height = 0; height < _cargoHeight; height++)
        {
            if (!_fullLines.Contains(height))
            {
                if (IsLineFull(height))
                {
                    _fullLines.Add(height);
                    if (_lineListParticleSystems != null)
                    {
                        _lineListParticleSystems[height].Play();
                    }
                    AudioManager.Instance.PlaySoundEffect(SoundEffectType.PLANESCORE);
                }
            }
        }
    }

    public bool IsLineFull(int height)
    {
        Collider[] hitWares = Physics.OverlapBox(transform.position + new Vector3(0, height + 0.5f, 0), new Vector3(_cargoMaxLenght, 1, _cargoMaxWidth) * 0.49f, Quaternion.identity, _wareLayerMask);
        return hitWares.Length >= _slots.Length;
    }

    public void DeactivateCargo()
    {
        Collider[] hitWares = Physics.OverlapBox(transform.position + new Vector3(0, _cargoHeight * 0.5f, 0), new Vector3(_cargoMaxLenght, _cargoHeight, _cargoMaxWidth) * 0.5f, Quaternion.identity, _wareLayerMask);
        for (int i = 0; i < hitWares.Length; i++)
        {
            hitWares[i].enabled = false;
        }
        for (int i = 0; i < _slots.Length; i++)
        {
            Collider[] colliders = _slots[i].GetComponents<Collider>();
            for (int j = 0; j < colliders.Length; j++)
            {
                Collider collider = colliders[j];
                collider.enabled = false;
            }
        }
    }

    public void ResetWares()
    {
        if (_placedWare != null)
        {
            foreach (Ware ware in _placedWare)
            {
                ware.transform.SetParent(ware.WarePoolContainer);
                ware.gameObject.SetActive(false);
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _slots = GetComponentsInChildren<CargoSlot>();
    }
#endif
}
