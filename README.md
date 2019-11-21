# AppendOnly

High speed C#  immutable append only list. 

## Install 

> dotnet add package AppendOnly

## Why another immutable collection class?

Firstly, existing immutable collections have a really difficult time allowing you to do add, delete's and updates that return new collections. They literally HAVE to copy stuff, a LOT. If you are only appending, LinkedList is your friend, and can appear to be "copied" instantaneously.

Secondly, the existing immutable libraries are complex and hard to use. They require builder classes and a good understanding of the real mechanics, otherwise you will have seriously bad performance problems.

Lastly: This class is totally safe to use in really really tight loops. As in, as tight as you can get.

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
        foreach(var move in new_moves.Items) DoSomethingFunky(move);
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

