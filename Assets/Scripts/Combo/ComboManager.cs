using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{

    [System.Serializable]   // this makes it appear in the Inspector
    public class ComboData
    {
        public int requiredValue;   // Points needed for this combo

        [ColorUsage(true, true)]
        public Color comboColor;    // The color for this combo
        public List<ComboImprovement> comboImprovements; // List of improvements for this combo
    }
    public static ComboManager Instance { get; private set; }

    [Header("Combo Settings")]
    [SerializeField] public List<ComboData> comboSequence = new List<ComboData>();
    [SerializeField] private SliderBar comboSliderBar;
    [SerializeField] private float decayRate = 5f;

    public int currentComboIndex = 0;
    private float currentComboValue = 0f;

    private GameObject player;
    private CinemachineImpulseSource impulseSource;


    // Track improvements tied to each combo index
    private Dictionary<int, List<ComboImprovement>> activeImprovementsByCombo = new Dictionary<int, List<ComboImprovement>>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (comboSequence.Count > 0 && comboSliderBar != null)
        {
            comboSliderBar.SetMaxValue(comboSequence[currentComboIndex].requiredValue);
            comboSliderBar.SetCurrentValue(0);
            comboSliderBar.SetBarColor(comboSequence[currentComboIndex].comboColor);
        }

        player = GameObject.FindGameObjectWithTag("Player");
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        // Decay combo value over time
        if (currentComboValue > 0 && currentComboIndex < comboSequence.Count)
        {
            currentComboValue -= decayRate * Time.deltaTime;
            currentComboValue = Mathf.Max(0, currentComboValue);
            comboSliderBar.SetCurrentValue(currentComboValue);
        }
    }

    public void AddComboProgress(int amount, bool showCombo)
    {
        if (comboSequence.Count == 0 || comboSliderBar == null) return;
        if (currentComboIndex >= comboSequence.Count) return;

        if (showCombo && player != null)
        {
            GameObject comboIndicator = ObjectPooler.Instance.SpawnFromPool(
                            "ComboIndicator",
                            player.transform.position,
                            Quaternion.identity
                        );

            if (comboIndicator != null)
            {
                comboIndicator.GetComponent<FloatingMessage>().ShowNumber(amount, true);
            }
        }

        currentComboValue += amount;
        int requiredValue = comboSequence[currentComboIndex].requiredValue;
        currentComboValue = Mathf.Min(currentComboValue, requiredValue);

        comboSliderBar.SetCurrentValue(currentComboValue);

        if (currentComboValue >= requiredValue)
        {
            CompleteCombo();
        }
    }

    private void CompleteCombo()
    {
        // Apply improvements of this combo
        ApplyImprovements(comboSequence[currentComboIndex].comboImprovements);

        currentComboIndex++;
        if (currentComboIndex < comboSequence.Count)
        {
            currentComboValue = 0;
            comboSliderBar.SetMaxValue(comboSequence[currentComboIndex].requiredValue);
            comboSliderBar.SetCurrentValue(0);
            comboSliderBar.SetBarColor(comboSequence[currentComboIndex].comboColor);
        }
        else
        {
            Debug.Log("All combos completed!");
        }
    }

    private void ApplyImprovements(List<ComboImprovement> improvements)
    {
        int key = currentComboIndex;

        // If this combo’s improvements were previously applied, deactivate them first
        if (activeImprovementsByCombo.TryGetValue(key, out var existing))
        {
            foreach (var imp in existing)
                imp.Deactivate();

            activeImprovementsByCombo.Remove(key);
        }

        // Activate fresh
        foreach (var imp in improvements)
            imp.Activate();

        // Store exactly this set (no duplicates)
        activeImprovementsByCombo[key] = new List<ComboImprovement>(improvements);
    }


    public void ResetAllCombos()
    {
        // 1. Deactivate ALL active improvements
        foreach (var kvp in activeImprovementsByCombo)
        {
            foreach (var imp in kvp.Value)
                imp.Deactivate();
        }
        activeImprovementsByCombo.Clear();

        // 2. Reset to the very first combo
        currentComboIndex = 0;
        currentComboValue = 0;

        // 3. Reset UI
        if (comboSequence.Count > 0 && comboSliderBar != null)
        {
            comboSliderBar.SetMaxValue(comboSequence[currentComboIndex].requiredValue);
            comboSliderBar.SetCurrentValue(0);
            comboSliderBar.SetBarColor(comboSequence[currentComboIndex].comboColor);
        }

        // 4. Trigger a Cinemachine impulse if available
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

    }



}
