// ----------------------------------------------
// DOTweenAsyncExtensions.cs
// Copyright (c) 2025 NigmaStudio
// 
// Provides extension methods to convert DOTween Tweens to UniTask.
// ----------------------------------------------

using Cysharp.Threading.Tasks;
using DG.Tweening;

/// <summary>
/// Contains extension methods for converting DOTween tweens to UniTask-based asynchronous tasks.
/// </summary>
public static class DOTweenAsyncExtensions
{
    /// <summary>
    /// Converts a Tween to a UniTask that completes when the tween completes.
    /// Cancels the task if the tween is killed before finishing.
    /// </summary>
    /// <param name="tween">The DOTween tween to await.</param>
    /// <returns>A UniTask that completes with no result when the tween completes.</returns>
    public static UniTask ToUniTask(this Tween tween)
    {
        var tcs = new UniTaskCompletionSource();

        tween.OnComplete(() => tcs.TrySetResult());
        tween.OnKill(() =>
        {
            if (!tween.IsComplete()) tcs.TrySetCanceled();
        });

        return tcs.Task;
    }

    /// <summary>
    /// Converts a Tween to a UniTask that completes with a specified result when the tween completes.
    /// Cancels the task if the tween is killed before finishing.
    /// </summary>
    /// <typeparam name="T">The type of result to return on completion.</typeparam>
    /// <param name="t">The DOTween tween to await.</param>
    /// <param name="result">The result to return upon tween completion.</param>
    /// <returns>A UniTask that completes with the given result when the tween completes.</returns>
    public static UniTask<T> ToUniTask<T>(this Tween t, T result)
    {
        var tcs = new UniTaskCompletionSource<T>();

        t.OnComplete(() => tcs.TrySetResult(result));
        t.OnKill(() =>
        {
            if (!t.IsComplete()) tcs.TrySetCanceled();
        });

        return tcs.Task;
    }
}
