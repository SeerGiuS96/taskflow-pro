# NgRx — My Notes

**Phase:** 4
**Status:** [ ] To write

---

## What problem does NgRx solve? (in my own words)

> Why not just use a service with a BehaviorSubject?
> When does shared state become a problem without NgRx?

---

## The 4 pieces and what each one does

**Store:**
> What is it? Where does the data live?

**Actions:**
> What is an action? Who dispatches them? What information do they carry?

**Reducers:**
> What is a reducer? Why must it be a pure function?
> What does "pure function" mean here?

**Effects:**
> What is an effect? Why can't we just put API calls in the reducer?

**Selectors:**
> What is a selector? Why are they memoized?

---

## The unidirectional data flow — draw it

> In your own words, trace what happens from a button click to the UI updating:

1. User clicks "Move task"
2. Component dispatches...
3. Store receives...
4. Reducer computes...
5. Effect...
6. Component re-renders because...

---

## Optimistic updates — the pattern I'm most proud of

> Explain what optimistic UI means and how the rollback works.

---

## What was confusing at first?

---

## How I would explain it in an interview

---

## Code example from this project

```typescript
// Paste board.actions.ts and the moveTask effect here
```
