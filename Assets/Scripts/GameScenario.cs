using UnityEngine;
using UnityEngine.SocialPlatforms;

[CreateAssetMenu]
public class GameScenario : ScriptableObject {

    [SerializeField]
    EnemyWave[] waves = {};
    
    public State Begin () => new State(this);
    
    [SerializeField, Range(0, 10)]
    int cycles = 1;
    
    [SerializeField, Range(0f, 1f)]
    float cycleSpeedUp = 0.5f;
    
    [System.Serializable]
    public struct State {

        GameScenario scenario;

        int cycle, index;

        private float tiemScale;

        EnemyWave.State wave;

        public State (GameScenario scenario) {
            this.scenario = scenario;
            cycle = 0;
            index = 0;
            tiemScale = 1f;
            Debug.Assert(scenario.waves.Length > 0, "Empty scenario!");
            wave = scenario.waves[0].Begin();
        }
        
        public bool Progress () {
            float deltaTime = wave.Progress(tiemScale *Time.deltaTime);
            while (deltaTime >= 0f) {
                if (++index >= scenario.waves.Length) {
                    if (++cycle >= scenario.cycles && scenario.cycles > 0)
                    {
                        return false;
                    }
                    index = 0;
                    tiemScale += scenario.cycleSpeedUp;
                }
                wave = scenario.waves[index].Begin();
                deltaTime = wave.Progress(deltaTime);
            }
            return true;
        }
    }
}