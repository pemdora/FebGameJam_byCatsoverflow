using UnityEngine;

[CreateAssetMenu(menuName = "Spaceships/Spaceship Color Scheme", fileName = "New Spaceship Color Scheme")]
public class SpaceshipColorScheme : ScriptableObject
{
    public Color[] colors;

    public Color GetRandomColor()
    {
        return colors[Random.Range(0, colors.Length)];
    }
}