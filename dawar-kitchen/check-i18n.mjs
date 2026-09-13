import fs from 'node:fs';
import path from 'node:path';

const root = path.resolve('src/assets/i18n');
const languages = ['en', 'ar'];
const files = fs.readdirSync(path.join(root, 'en')).filter(file => file.endsWith('.json')).sort();
const domainFiles = ['menu', 'reservations', 'auth', 'contact', 'payment'];
const homeSections = ['hero', 'about', 'blog', 'chefs', 'locations'];

function deepMerge(base, override) {
  const result = { ...base };
  for (const [key, value] of Object.entries(override)) {
    if (value && typeof value === 'object' && !Array.isArray(value) && result[key] && typeof result[key] === 'object') {
      result[key] = deepMerge(result[key], value);
    } else {
      result[key] = value;
    }
  }
  return result;
}

function flatten(value, prefix = '', output = new Map()) {
  for (const [key, child] of Object.entries(value)) {
    const fullKey = prefix ? `${prefix}.${key}` : key;
    if (child && typeof child === 'object' && !Array.isArray(child)) flatten(child, fullKey, output);
    else output.set(fullKey, child);
  }
  return output;
}

function placeholders(value) {
  return [...value.matchAll(/{{\s*([^}\s]+)\s*}}/g)].map(match => match[1]).sort();
}

function parse(file) {
  try {
    return JSON.parse(fs.readFileSync(file, 'utf8'));
  } catch (error) {
    throw new Error(`${file}: invalid JSON (${error.message})`);
  }
}

if (process.argv.includes('--fix')) {
  for (const lang of languages) {
    const languageRoot = path.join(root, lang);
    const commonPath = path.join(languageRoot, 'common.json');
    const common = parse(commonPath);
    const homePath = path.join(languageRoot, 'home.json');
    const homeData = parse(homePath);
    const home = {};
    for (const section of homeSections) {
      home[section] = deepMerge(common[section] ?? {}, homeData[section] ?? {});
      delete common[section];
    }
    fs.writeFileSync(homePath, `${JSON.stringify(home, null, 2)}\n`);

    for (const domain of domainFiles) {
      const domainPath = path.join(languageRoot, `${domain}.json`);
      const domainData = parse(domainPath);
      const merged = deepMerge(common[domain] ?? {}, domainData[domain] ?? {});
      fs.writeFileSync(domainPath, `${JSON.stringify({ [domain]: merged }, null, 2)}\n`);
      delete common[domain];
    }
    fs.writeFileSync(commonPath, `${JSON.stringify(common, null, 2)}\n`);
  }
}

const errors = [];
const all = new Map();
for (const language of languages) {
  const languageRoot = path.join(root, language);
  const languageKeys = new Map();
  for (const file of files) {
    const data = parse(path.join(languageRoot, file));
    const keys = flatten(data);
    for (const [key, value] of keys) {
      if (languageKeys.has(key)) errors.push(`${language}: duplicate key ${key}`);
      languageKeys.set(key, value);
    }
  }
  all.set(language, languageKeys);
  for (const [key, value] of languageKeys) {
    if (value === null || value === '') errors.push(`${language}: empty value ${key}`);
  }
}

const en = all.get('en');
const ar = all.get('ar');
for (const key of [...en.keys()].filter(key => !ar.has(key))) errors.push(`AR missing: ${key}`);
for (const key of [...ar.keys()].filter(key => !en.has(key))) errors.push(`EN missing: ${key}`);
for (const key of [...en.keys()].filter(key => ar.has(key))) {
  const enValue = en.get(key);
  const arValue = ar.get(key);
  if (typeof enValue === 'string' && typeof arValue === 'string') {
    const enParams = placeholders(enValue).join(',');
    const arParams = placeholders(arValue).join(',');
    if (enParams !== arParams) errors.push(`Interpolation mismatch: ${key} (en: ${enParams || 'none'}, ar: ${arParams || 'none'})`);
  }
}

if (errors.length) {
  console.error(errors.join('\n'));
  process.exitCode = 1;
} else {
  console.log(`i18n parity passed: ${en.size} keys in en and ar across ${files.length} files`);
}
