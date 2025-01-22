using System.Collections.Generic;
using UnityEngine;

public class CharPooler : MonoBehaviour
{
    Queue<Character> normies;
    Queue<Character> champions;
    Queue<Character> enemyNormies;
    Queue<Character> bigEnemies;
    
    public void Init()
    {
        // TODO: instantiate required characters and put them in the queue for. the amount can be calculated from waves list for enemies.
        // and for the mobs it should be proportional to enemies count
        normies = new Queue<Character>(100);
        champions = new Queue<Character>(100);
        enemyNormies = new Queue<Character>(100);
        bigEnemies = new Queue<Character>(100);
    }

    public Character GetChar(Character charPrefab, Vector3 pos, Quaternion rot)
    {
        Queue<Character> charQueue;
        switch (charPrefab.charType)
        {
            case CharacterType.normie:
                charQueue = normies;
                break;
            case CharacterType.champion:
                charQueue = champions;
                break;
            case CharacterType.enemyNormie:
                charQueue = enemyNormies;
                break;
            case CharacterType.enemyBig:
                charQueue = bigEnemies;
                break;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(charPrefab.charType), charPrefab.charType, null);
        }
        if (charQueue.Count > 0)
        {
            var n =  charQueue.Dequeue();
            n.gameObject.SetActive(true);
            n.transform.position = pos;
            n.transform.rotation = rot;
            return n;
        }

        return Instantiate(charPrefab, pos, rot, transform);
    }
    
    public void DestroyChar(Character c)
    {
        Queue<Character> charQueue = c.charType switch
        {
            CharacterType.normie => normies,
            CharacterType.champion => champions,
            CharacterType.enemyNormie => enemyNormies,
            CharacterType.enemyBig => bigEnemies,
            _ => throw new System.ArgumentOutOfRangeException(nameof(c.charType), c.charType, null)
        };
        c.gameObject.SetActive(false);
        charQueue.Enqueue(c);
    }
}
