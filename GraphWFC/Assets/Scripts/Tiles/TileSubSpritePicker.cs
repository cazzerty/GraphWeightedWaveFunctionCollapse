using UnityEngine;
using Random = UnityEngine.Random;

namespace Tiles
{
    public class TileSubSpritePicker : MonoBehaviour
    {
        [SerializeField] private Sprite[] sprites;

        [SerializeField] private int firstOffset = 6;

        [SerializeField] private SpriteRenderer spriteRenderer;
        // Start is called before the first frame update
        void Start()
        {
            if (!spriteRenderer) { spriteRenderer = GetComponent<SpriteRenderer>();}
            if(sprites.Length == 0){return;}

            PickSprite();
        }

        private void PickSprite()
        {
            int rand = Random.Range(0, firstOffset);
            if (rand != 0) { return;}
            rand = Random.Range(0, sprites.Length);
            spriteRenderer.sprite = sprites[rand];
        }
    }
}
