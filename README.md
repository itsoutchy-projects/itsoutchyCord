# itsoutchyCord
A custom discord client designed for ease of use.  
It's *much* faster thanks to it using Webview2 (*should* probably work on other OSes, however you'll have to compile and test it yourself as I only have windows)

## Benefits
- Having all 3 types of clients (stable, ptb, canary) packed into one
- Custom Javascript and CSS injection (incomplete)
- Loads so much faster than the official client due to there being no update utility
- Install is a *lot* smaller

## How to change settings?
Find the settings.txt file, then edit the property (do not put a space before or after the equals sign)
For example, say you wanted to change the client from stable to canary, this would be the result
```
client=canary
```

## TODO:
- [ ] Add support for BetterDiscord plugins  
- [ ] Fix theme injection not working (for now a little fix is to use javascript to create a `<style>` element and set its `innerText` property to the css code)  
- [ ] Add vencord (working on that)  