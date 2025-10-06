# USSTransition & USSSequence API Documentation

## USSTransition

### Static Factory Methods

**Transform & Position**
- `USSTransition.Opacity(VisualElement element, float target, float duration)` - Fade opacity
- `USSTransition.Scale(VisualElement element, Vector2 target, float duration)` - Scale element
- `USSTransition.Rotate(VisualElement element, float degrees, float duration)` - Rotate element
- `USSTransition.Translate(VisualElement element, Vector2 target, float duration)` - Translate position
- `USSTransition.TranslateX(VisualElement element, float target, float duration)` - Translate X only
- `USSTransition.TranslateY(VisualElement element, float target, float duration)` - Translate Y only

**Size**
- `USSTransition.Width(VisualElement element, float target, float duration)` - Animate width
- `USSTransition.Height(VisualElement element, float target, float duration)` - Animate height

**Color**
- `USSTransition.BackgroundColor(VisualElement element, Color target, float duration)` - Background color
- `USSTransition.Color(VisualElement element, Color target, float duration)` - Text color
- `USSTransition.BorderColor(VisualElement element, Color target, float duration)` - Border color

**Spacing**
- `USSTransition.MarginLeft/Top/Right/Bottom(VisualElement element, float target, float duration)` - Margins
- `USSTransition.PaddingLeft/Top/Right/Bottom(VisualElement element, float target, float duration)` - Padding

**Border**
- `USSTransition.BorderWidth(VisualElement element, float target, float duration)` - Border width
- `USSTransition.BorderRadius(VisualElement element, float target, float duration)` - Border radius

**Position**
- `USSTransition.Top/Bottom/Left/Right(VisualElement element, float target, float duration)` - Absolute position

**Flex**
- `USSTransition.FlexGrow(VisualElement element, float target, float duration)` - Flex grow
- `USSTransition.FlexShrink(VisualElement element, float target, float duration)` - Flex shrink

### Instance Methods (Chaining)

**Configuration**
- `.SetDelay(float delaySeconds)` - Delay before starting
- `.SetEase(EasingMode mode)` - Set easing function (EasingMode enum)
- `.SetFrom(Action<VisualElement> fromAction)` - Set starting value

**Callbacks**
- `.OnStart(Action callback)` - Called when transition starts
- `.OnUpdate(Action callback)` - Called each frame during transition
- `.OnComplete(Action callback)` - Called when transition completes

**Playback**
- `.Play()` - Start the transition (returns this for chaining)
- `.Pause()` - Pause the transition
- `.Resume()` - Resume from pause
- `.Kill()` - Stop and reset transition
- `.Complete()` - Jump to end and call OnComplete

### Properties
- `.Duration` - Total duration including delay (read-only)

---

## USSSequence

### Static Factory
- `USSSequence.Create()` - Create new sequence

### Sequencing Methods

**Adding Transitions**
- `.Append(USSTransition transition)` - Add transition after previous
- `.Join(USSTransition transition)` - Add transition simultaneously with previous
- `.Insert(float atTime, USSTransition transition)` - Add transition at specific time

**Timing**
- `.AppendInterval(float interval)` - Add delay after current duration
- `.PrependInterval(float interval)` - Add delay at start, shifting everything forward

**Callbacks**
- `.AppendCallback(Action callback)` - Execute callback after current duration
- `.InsertCallback(float atTime, Action callback)` - Execute callback at specific time

**Configuration**
- `.SetLoops(int loopCount)` - Number of times to repeat (0 = infinite)
- `.OnStart(Action callback)` - Called when sequence starts
- `.OnUpdate(Action callback)` - Called each frame
- `.OnComplete(Action callback)` - Called when sequence completes

**Playback**
- `.Play()` - Start the sequence
- `.Pause()` - Pause all active transitions
- `.Resume()` - Resume from pause
- `.Kill()` - Stop and kill all transitions
- `.Complete()` - Jump to end

---

## Extension Methods (USSTransitionExtensions)

Convenience methods on VisualElement:

- `.DOFade(float target, float duration)` - Fade opacity
- `.DOScale(Vector2 target, float duration)` - Scale
- `.DOScale(float target, float duration)` - Uniform scale
- `.DORotate(float degrees, float duration)` - Rotate
- `.DOMove(Vector2 target, float duration)` - Translate
- `.DOMoveX(float target, float duration)` - Translate X
- `.DOMoveY(float target, float duration)` - Translate Y
- `.DOColor(Color target, float duration)` - Text color
- `.DOBackgroundColor(Color target, float duration)` - Background color
- `.DOWidth(float target, float duration)` - Width
- `.DOHeight(float target, float duration)` - Height

---

## EasingMode Values

Available easing functions:
- `EasingMode.Ease` (default)
- `EasingMode.Linear`
- `EasingMode.EaseIn`, `EaseOut`, `EaseInOut`
- `EasingMode.EaseInSine`, `EaseOutSine`, `EaseInOutSine`
- `EasingMode.EaseInCubic`, `EaseOutCubic`, `EaseInOutCubic`
- `EasingMode.EaseInCirc`, `EaseOutCirc`, `EaseInOutCirc`
- `EasingMode.EaseInElastic`, `EaseOutElastic`, `EaseInOutElastic`
- `EasingMode.EaseInBack`, `EaseOutBack`, `EaseInOutBack`
- `EasingMode.EaseInBounce`, `EaseOutBounce`, `EaseInOutBounce`

---

## Usage Examples

### Simple Transition
```csharp
// Fade element out
element.DOFade(0f, 0.5f).Play();
```

### With Easing and Callback
```csharp
element.DOScale(1.5f, 0.3f)
    .SetEase(EasingMode.EaseOutBack)
    .OnComplete(() => Debug.Log("Done"))
    .Play();
```

### Multiple Properties Simultaneously
```csharp
USSSequence.Create()
    .Append(element.DOScale(1.2f, 0.5f).SetEase(EasingMode.EaseOutBack))
    .Join(element.DOFade(1f, 0.5f))
    .Play();
```

### Sequential Animations
```csharp
USSSequence.Create()
    .Append(element1.DOFade(1f, 0.3f))
    .AppendInterval(0.2f)
    .Append(element2.DOScale(1f, 0.3f))
    .AppendCallback(() => Debug.Log("Halfway"))
    .Append(element3.DOMove(new Vector2(100, 0), 0.4f))
    .OnComplete(() => Debug.Log("All done"))
    .Play();
```

### Looping Sequence
```csharp
USSSequence.Create()
    .Append(element.DOScale(1.2f, 0.5f))
    .Append(element.DOScale(1f, 0.5f))
    .SetLoops(3)
    .Play();
```

### Insert at Specific Time
```csharp
USSSequence.Create()
    .Append(element1.DOFade(1f, 1f))
    .Insert(0.5f, element2.DOScale(1.5f, 0.5f))
    .Play();
```

### Complex Sequence
```csharp
USSSequence.Create()
    .Append(topRibbon.DOScale(1f, 0.5f).SetEase(EasingMode.EaseOutBack).SetDelay(0.2f))
    .Join(topRibbon.DOFade(1f, 0.5f).SetEase(EasingMode.EaseOutBack).SetDelay(0.2f))
    .AppendInterval(0.3f)
    .Append(titleText.DOFade(1f, 0.4f))
    .Join(titleText.DOScale(1f, 0.4f))
    .AppendCallback(() => ShowNextElement())
    .OnComplete(() => Debug.Log("Intro complete"))
    .Play();
```

### Playback Control
```csharp
var transition = element.DOFade(0f, 1f).Play();

// Control playback
transition.Pause();
transition.Resume();
transition.Kill();
transition.Complete();
```

---

## Notes

- All transitions use Unity's native USS (Unity Style Sheets) transition system
- Transitions are hardware-accelerated where supported
- The `USSTransitionRunner` MonoBehaviour is automatically created and persists across scenes
- Use `.Join()` in sequences to run multiple transitions simultaneously
- Use `.Append()` to run transitions one after another
- Easing functions match the CSS/USS easing specification