using UnityEngine;

/*Classe que vai conter todos os assets do nosso projecto para podermos referenciar em outros scripts*/
public class GameAssets : MonoBehaviour
{
    //static significa que pode ser acedido em qualquer outro lugar do nosso codigo
    private static GameAssets instance;
    //referencia à nossa texture do pipehead e restantes prefabs
    public Sprite PipeHeadSprite;
    public Transform PipeHead_Pref;
    public Transform Pipe_Pref;
    public Transform Ground_Pref;
    public Transform Cloud1_Pref;
    public Transform Cloud2_Pref;
    public Transform Cloud3_Pref;

    private void Awake()
    {
        instance = this;
    }
    /*Função que devolve a instance da classe (GETTER FUCTION)*/
    public static GameAssets GetInstance() 
    {
        return instance;
    }
}
