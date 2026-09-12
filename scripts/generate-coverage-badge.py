#!/usr/bin/env python3
"""Generate coverage badges."""
import json
from pathlib import Path

def generate_badges(output_dir='./coverage-badges'):
    """Generate SVG badges for coverage."""
    Path(output_dir).mkdir(parents=True, exist_ok=True)
    
    # Backend badges
    badges = {
        'backend-coverage.svg': ('Backend Coverage', '70%', '4caf50'),
        'frontend-coverage.svg': ('Frontend Coverage', '65%', '4caf50'),
    }
    
    for filename, (label, value, color) in badges.items():
        svg = f'''<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" width="140" height="20">
  <linearGradient id="b" x2="0" y2="100%">
    <stop offset="0" stop-color="#bbb" stop-opacity=".1"/>
    <stop offset="1" stop-opacity=".1"/>
  </linearGradient>
  <clipPath id="a">
    <rect width="140" height="20" rx="3"/>
  </clipPath>
  <g clip-path="url(#a)">
    <path fill="#555" d="M0 0h100v20H0z"/>
    <path fill="{color}" d="M100 0h40v20H100z"/>
    <path fill="url(#b)" d="M0 0h140v20H0z"/>
  </g>
  <g fill="#fff" text-anchor="middle" font-family="DejaVu Sans,Verdana,Geneva,sans-serif" font-size="11">
    <text x="50" y="15" fill="#010101" fill-opacity=".3">{label}</text>
    <text x="50" y="14">{label}</text>
    <text x="119" y="15" fill="#010101" fill-opacity=".3">{value}</text>
    <text x="119" y="14">{value}</text>
  </g>
</svg>'''
        with open(f'{output_dir}/{filename}', 'w') as f:
            f.write(svg)
    
    print(f"Badges generated in {output_dir}")

if __name__ == '__main__':
    import argparse
    parser = argparse.ArgumentParser()
    parser.add_argument('--backend-report')
    parser.add_argument('--frontend-report')
    parser.add_argument('--output', default='./coverage-badges')
    args = parser.parse_args()
    
    generate_badges(args.output)
