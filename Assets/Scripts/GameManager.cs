using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton Pattern
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeComponents(); // Ensure all scripts are linked
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Script References
    public InputManager InputManager { get; private set; }
    public TowerPlacer TowerPlacer { get; private set; }
    public GoldManager GoldManager { get; private set; }
    public DamageManager DamageManager { get; private set; }
    public OutlineManager OutlineManager { get; private set; }
    #endregion

    private void InitializeComponents()
    {
        InputManager = GetComponent<InputManager>();
        TowerPlacer = GetComponent<TowerPlacer>();
        GoldManager = GetComponent<GoldManager>();
        DamageManager = GetComponent<DamageManager>();
        OutlineManager = GetComponent <OutlineManager> ();

        if (InputManager == null) Debug.LogError("InputManager is missing from GameManager!");
        if (TowerPlacer == null) Debug.LogError("TowerPlacer is missing from GameManager!");
        if (GoldManager == null) Debug.LogError("GoldManager is missing from GameManager!");
        if (DamageManager == null) Debug.LogError("DamageManager is missing from GameManager!");
        if (OutlineManager == null) Debug.LogError("OutlineManager is missing from GameManager!");
    }
}
