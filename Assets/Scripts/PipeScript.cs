using System.Collections;
using UnityEngine;

public class PipeScript : MonoBehaviour
{
    public GameObject player;
    public Rigidbody2D rb;
    public AudioClip collisionSound;
    private AudioSource _audioSource;
    public float destroyDelay = 5.0f;
    public float fadeOutDuration = 1.0f;

    public PlayerMovements playerMovements;

    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = new Vector2(25, 0);

        // Add and configure AudioSource
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.clip = collisionSound;

        // Store the AudioSource in a private variable
        _audioSource = audioSource;


        StartCoroutine(FadeOutAndDestroy());
    }

    // Update is called once per frame
    void Update()
    {

    }

    // OnCollisionEnter2D is called when this collider/rigidbody starts
    // touching another rigidbody/collider
    void OnCollisionEnter2D(Collision2D collision)
    {
        _audioSource.Play();
    }

    private IEnumerator FadeOutAndDestroy()
    {
        yield return new WaitForSeconds(destroyDelay);

        float elapsedTime = 0;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0); // Set target alpha to 0

        while (elapsedTime < fadeOutDuration)
        {
            spriteRenderer.color = Color.Lerp(originalColor, targetColor, elapsedTime / fadeOutDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteRenderer.color = targetColor;
        Destroy(gameObject);
    }

}
