using UnityEngine;

public class RipplePostProcessor : MonoBehaviour
{
    public Material RippleMaterial;
    public float MaxAmount = 50f;

    [Range(0, 1)]
    public float Friction = .9f;

    private float Amount = 0f;

    void Update()
    {
        //if (Input.GetMouseButton(0))
        //{
        //    this.Amount = this.MaxAmount;
        //    Vector3 pos = Input.mousePosition;
        //    this.RippleMaterial.SetFloat("_CenterX", pos.x);
        //    this.RippleMaterial.SetFloat("_CenterY", pos.y);
        //}

        this.RippleMaterial.SetFloat("_Amount", this.Amount);
        this.Amount *= this.Friction;
    }

    public void CreateRippleEffect(Vector3 position)
    {
        var pixelPosition = Camera.main.WorldToScreenPoint(position);
        this.Amount = this.MaxAmount;
        //Vector3 pos = Input.mousePosition;
        this.RippleMaterial.SetFloat("_CenterX", pixelPosition.x);
        this.RippleMaterial.SetFloat("_CenterY", pixelPosition.y);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        Graphics.Blit(src, dst, this.RippleMaterial);
    }
}