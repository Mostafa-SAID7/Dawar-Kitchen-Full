# RTL Implementation Quick Reference

## For Developers

### When Adding New Components

#### Flex Containers with Icon + Text
```html
<!-- Template -->
<div class="flex items-center gap-2 rtl:flex-row-reverse">
  <iconify-icon icon="solar:arrow-right-linear" class="rtl:rotate-180"></iconify-icon>
  <span>Label Text</span>
</div>
```

#### Buttons with Icons
```html
<button class="flex items-center gap-2 rtl:flex-row-reverse">
  {{ 'action.label' | translate }}
  <iconify-icon icon="solar:arrow-right-linear" width="16" class="rtl:rotate-180"></iconify-icon>
</button>
```

#### Forms & Labels
```html
<label class="text-xs font-medium text-neutral-400 uppercase text-start">
  {{ 'form.label' | translate }}
</label>
<input type="text" class="nn-input" />
<!-- CSS already handles RTL direction -->
```

#### Positioned Dropdowns/Modals
```html
<!-- Use logical properties -->
<div class="absolute end-0 top-full ...">
  <!-- In RTL: appears on left via inset-inline-end: 0 -->
</div>
```

#### Menu Items in Grid
```html
<div class="flex items-start justify-between gap-4 rtl:flex-row-reverse">
  <div class="flex-1">Item Name</div>
  <div>Price</div>
</div>
```

---

## Critical Patterns to Remember

| Pattern | Usage | Why |
|---------|-------|-----|
| `rtl:flex-row-reverse` | Flex containers with icon + text | Reverses order in RTL |
| `rtl:rotate-180` | Directional icons (arrows, chevrons) | Mirrors icon direction |
| `end-0` / `start-0` | Positioned elements | Logical positioning |
| `border-s` / `border-e` | Borders | Logical borders |
| `text-start` | Labels, descriptions | Always start from beginning |
| `font-cairo` | Arabic headings (optional) | Already in tailwind.config |

---

## Global RTL Rules (Already Applied)

**Don't override these:**
- ✅ `html[dir="rtl"]` — sets `direction: rtl; text-align: start;`
- ✅ Form inputs — inherit `direction: rtl; text-align: start;`
- ✅ Select dropdowns — chevron positioned correctly in RTL

**Don't add:**
- ❌ `body.rtl .flex { flex-direction: row-reverse; }` — Too aggressive
- ❌ `body.rtl button { direction: rtl; }` — Breaks button layouts
- ❌ Physical properties like `right-0`, `left-0` — Use `end-0`, `start-0` instead

---

## Testing RTL Changes

### In Browser
1. Open dev tools
2. Run in console:
   ```javascript
   // Switch to Arabic
   document.documentElement.lang = 'ar';
   document.documentElement.dir = 'rtl';
   document.documentElement.classList.add('rtl');
   
   // Switch back to English
   document.documentElement.lang = 'en';
   document.documentElement.dir = 'ltr';
   document.documentElement.classList.remove('rtl');
   ```

3. Or use the language toggle button in header (if logged in)

### What to Check
- ✅ Header: logo on left in Arabic, right in English
- ✅ Dropdowns: positioned on correct side
- ✅ Cart drawer: slides from correct edge
- ✅ Icons: arrows point correct direction
- ✅ Forms: labels and inputs aligned correctly
- ✅ Text: doesn't overflow, readable at all sizes
- ✅ Font: Cairo in Arabic, Open Sans in English

---

## Common Issues & Fixes

### Issue: Flex container items in wrong order
**Fix:** Add `rtl:flex-row-reverse`
```html
<div class="flex gap-2 rtl:flex-row-reverse">
  <!-- Items now reverse order in RTL -->
</div>
```

### Issue: Icon points wrong direction in Arabic
**Fix:** Add `rtl:rotate-180`
```html
<iconify-icon icon="solar:arrow-right-linear" class="rtl:rotate-180"></iconify-icon>
```

### Issue: Dropdown appears on wrong side
**Fix:** Use `end-0` instead of `right-0`
```html
<div class="absolute end-0 top-full">
  <!-- Automatically on right in LTR, left in RTL -->
</div>
```

### Issue: Text doesn't align properly in forms
**Fix:** Add `text-start` to labels, ensure `nn-input` class is used
```html
<label class="text-start">Label</label>
<input class="nn-input" />
```

### Issue: Brand name "Dawar Kitchen" not in right font
**Fix:** It's handled globally — Cairo is applied to `font-['Forum']` in RTL
- No changes needed; it's automatic

---

## Build & Deploy

### Development
```bash
npm run dev  # Runs on port 5000
```

### Production Build
```bash
npm run build --configuration production
# Output: dist/dawar-kitchen/browser/
```

### Vercel Deploy
- ✅ Vercel.json already configured with root directory
- ✅ No special setup needed for RTL
- ✅ Just push to main branch

---

## Resources

- **Full Details:** `RTL_IMPLEMENTATION_SUMMARY.md`
- **LanguageService:** `src/app/shared/services/language.service.ts`
- **Global Styles:** `src/styles.css` (RTL section at end)
- **Tailwind Config:** `tailwind.config.js` (rtl/ltr plugins)

---

## Checklist Before Commit

When adding new RTL-affected components:
- [ ] Added `rtl:flex-row-reverse` to flex containers with icon + text
- [ ] Added `rtl:rotate-180` to all directional icons
- [ ] Used `end-0`/`start-0` instead of `right-0`/`left-0`
- [ ] Used `text-start` for labels and descriptions
- [ ] Tested in both LTR (English) and RTL (Arabic)
- [ ] No hardcoded `left`/`right` in CSS classes

---

**Last Updated:** September 13, 2026  
**Status:** Production Ready ✅
