# AppendOnly

High speed C#  immutable append only list.  (update:23 Sep 24; not recommended to use. See notes at bottom for rationale and alternatives.)

## Install 

> dotnet add package AppendOnly

## Why another immutable collection class?

Firstly, existing immutable collections have a really difficult time allowing you to do add, delete's and updates that return new collections. They literally HAVE to copy stuff, a LOT. If you are only appending, LinkedList is your friend, and can appear to be "copied" instantaneously.

Secondly, the existing immutable libraries are complex and hard to use. They require builder classes and a good understanding of the real mechanics, otherwise you will have seriously bad performance problems.

Lastly: This class is totally safe to use in really really tight loops. As in, as tight as you can get.

## (potentially n squared / 2 better)

AppendOnly uses A linkedList internally, and never copies the collection items when creating a new "immutable" copy. It simply returns a new collection class that shares the same internal "mutable" linkedList, but does not expose any methods to mutate it, only exposing an interator that iterates through the first (n) items in the collection. 

So as the collection grows bigger, earlier (older) instances of the collection class, will never see (expose via the enumerator) the new entries appended to the end of the list. After adding 1000 new "transactions" to a appendOnly transactionCollection, you effectively have (if you kept the references to each newly created collection) 1000 pointers to collections each containing, 1, 2, 3, 4 etc items, but with close to zero additional memory overhead.

If using the other immutable libraries with builders, most of them would give you at minimum 500 000 instances of objects.

I may be wrong, it's a starting point, let me know if you use this. I'll be create more collections along similar lines.

## usage

```csharp
    var moves = new AppendOnlyList<Moves>();
    var new_moves = new AppendOnlyList<Moves>();

    Debug.Assert(moves.Length == 0);

    // add 10 random moves 
    for(int i=0; i<10; i++) new_moves = new_moves.Add(randomMove());

    // original moves still has no items even though new_list has added an item to the List.
    Debug.Assert(moves.Length == 0);

    // new_list has 10 item in it's collection
    Debug.Assert(new_moves.Length == 10);


    // this list is safe to iterate over multiple times and is threadsafe
    // code below shows iterating over the whole list ten times, something you would normally only do against
    // an enumerable if you've cached it, i.e. created a local copy.
    // I know this doesn't look like it's doing much, but this is really important. 
    // also, it's safe to do this WHILE other threads are busy adding to the same underlying collection, 
    // something that is a massive NO NO in threading world. Here, it's totally safe.
    for(int i = 0; i<10; i++)
    {
        foreach(var move in new_moves.Items) await DoSomethingFunkyAsync(move);
    }
```

In the code above, `new_moves` and the original `moves` share the same underlying linked list. When a new copy of the AppendOnlyList is created, there
is NO new clones made, only a point to the head of the list and the Length is maintained.

> This is EXTREMELY FAST, as in, NO COPY is made, so it's infinately faster than any other immutable collection that creates copies.

> And it's threadsafe to iterate over the collection WHILE it is being enumerated from other threads! BOOM! ... or more specifically "no Boom!".

## Project status

this is version `0.1.3` which means it's not ready for production use. It's a proof of concept, and I need to write some threading tests to prove the claims above, and then ask my peers to review and let me know.

I also want to do a speed comparison between this and other collections as well as a side by side comparison.

if you like this package, take a look at the actual implementation it's literally 2 classes, `AppendOnlyList` and `LinkedListExtensions`, and they're tiny.

let me know what you think?

:D

Any questions, contact me on, 

```
Alan Hemmings, 
twitter  : @snowcode 
LinkedIn : https://www.linkedin.com/in/goblinfactory
www      : https://goblinfactory.co.uk
```

# Update 23 September 2024 (need to revisit this project) 

#### It's been a few years since I originally wrote this; 

...and didnt use it much, and well, forgot about it. Things (C#) have moved on a lot since I wrote this, and while there's a lot of changes in C#, none of them have really simplified things a lot. Unfortunately I think my thoughts above, about when to use this package, in light of new language changes, might be rubbish, and may have been...quite faulty. Test are DEFININITELY be necessary before doing anything further. Here's some notes from a quick chat with an AI C# Architect, suggesting a revist to this project. TBH I think this project might still be warranted, but perhaps as a DSL wrapper (kind of simple factory class) around the different ways to properly create appendonly type structures. Nuff said; here's the feedback;

</hr>

(draft notes, needs to be tested, and confirmed!)

## Q: Is there a more efficient append only memory structure for C# that can be efficiently enumerated?

## : A: Yes

Yes, in C#, you can use `System.Collections.Generic.ArrayBuffer<T>` or `System.Buffers.ArrayPool<T>` for more efficient append-only and memory-efficient structures that can be enumerated efficiently. However, if you want a more practical, efficient append-only structure, consider these options:

1. `ImmutableArray<T>.Builder`
- An append-only structure provided by the `System.Collections.Immutable` package.
- It allows efficient appending and is mutable while building, but becomes immutable once converted to an `ImmutableArray<T>`.

```cs
var builder = ImmutableArray.CreateBuilder<int>();
builder.Add(1);
builder.Add(2);
var immutableArray = builder.ToImmutable();
```

2. `Memory<T>` and `Span<T>` with `ArrayPool<T>`
- Efficient for handling large data blocks with minimal memory allocations.
- You manage memory yourself, which avoids frequent reallocations compared to `List<T>`.
- Suitable if you want more control over memory usage but require careful management.

```cs
var pool = ArrayPool<int>.Shared;
int[] array = pool.Rent(1000); // Request an array from the pool
// Append and manage your data here
pool.Return(array); // Return to the pool when done
```

3. `ValueListBuilder<T>` (From System.Buffers)
- Provides an efficient way to build collections in a stack-allocated fashion.
- Especially useful when you know the data size is bounded and want high performance without heap allocations.
```cs
Span<int> initialSpan = stackalloc int[16]; 
var valueListBuilder = new ValueListBuilder<int>(initialSpan);

valueListBuilder.Append(1);
valueListBuilder.Append(2);
ReadOnlySpan<int> span = valueListBuilder.AsSpan();
```

4. `Sequence<T>` from System.IO.Pipelines
- Suited for handling a sequence of data efficiently in an append-only manner.
- Good for scenarios requiring high-performance I/O operations and streaming data.

## Summary

- `List<T>`: Good for appending (amortized `O(1)`) and provides fast enumeration due to contiguous memory.
- `ImmutableArray<T>.Builder`: Efficient for appending and then making immutable.
- `Memory<T>` with `ArrayPool<T>`: Great for reducing allocations.
- `ValueListBuilder<T>`: Efficient stack-based building when size is known. Stack-based, suitable for temporary, efficient collections.
- `Sequence<T>`: For high-throughput scenarios needing efficient appending.

Choose based on your specific performance and memory requirements.

# what about linkedlist?

ChatGPT said;

`LinkedList<T>` in C# is an option for an append-only memory structure, but it's generally less memory-efficient and slower for enumeration compared to other structures due to the following reasons:

### Considerations with `LinkedList<T>`:

 - **Memory Overhead**: Each node in a `LinkedList<T>` contains references to both the next and previous nodes, which introduces significant memory overhead compared to contiguous structures like arrays or `List<T>`.
 - **Cache Inefficiency**: Unlike arrays or `List<T>` which are stored in contiguous memory, `LinkedList<T>` nodes are scattered across the heap. This makes it less cache-friendly, leading to slower iteration and access times.
- **Enumeration Performance**: While `LinkedList<T>` is efficient for append operations (`O(1)` for adding to the end), its enumeration performance is typically slower than arrays or lists because it lacks the cache locality advantage.

### When to Use `LinkedList<T>`:

- When you need frequent insertions or deletions at both ends or in the middle.
- If you want to avoid shifting elements like in `List<T>`.

#### In summary, `LinkedList<T>` can be used for append-only scenarios but is generally not the most efficient choice for enumeration or memory usage compared to `List<T>` or other modern data structures.
