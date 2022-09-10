using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DarkRiftRPG
{
    public class SpawnEnemy : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject Enemy;
        public ushort counter = 0;
        
        public void SpawnEnemyFunc()
        {
            GameObject enemy = Instantiate(Enemy, Enemy.transform.position, Enemy.transform.rotation);
            EnemyAi changeId = enemy.GetComponent<EnemyAi>();
            changeId.enemyID = counter;

            PlayerManager.Instance.Enemies.Add(counter, enemy);
            
            EnemySpawnData enemySpawn = new EnemySpawnData(counter, Enemy.transform.position, Enemy.transform.rotation);
            ServerManager.Instance.SendToAll(Tags.SpawnEnemy, enemySpawn);
            counter++;
        }
    }
}