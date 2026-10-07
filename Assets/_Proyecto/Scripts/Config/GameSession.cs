/*
 * GameSession.cs
 *
 * Una clase estatica que guarda lo elegido en el menú(cantidad de jugadores y dificultad)
 *
 * Autor: Pablo G. Ruiz Orozco (Scuuz)
 * Fecha: 06/10/2026
 * Proyecto: TanquesPrototipo - Programación 2
 */


public static class GameSession
{
    public static int PlayerCount {get; set;} = 1;
    public static DifficultyConfig Difficulty {get; set;}

    public static DifficultyConfig GetDifficulty(DifficultyConfig fallback)
    {
        return Difficulty != null ? Difficulty : fallback;
    }
}
