"""
generate_heightmap.py

Generates a 1024x1024 grayscale heightmap PNG that resembles Mars terrain.
Features:
  - Rolling hills via multi-octave sine-based pseudo-Perlin noise
  - Craters of varying sizes with raised rims and depressed floors
  - Ridge / mountain chains
  - Output range 0-255 grayscale

Dependencies: Pillow (PIL), math, random
"""

import math
import random
from PIL import Image

SIZE = 1024
SEED = 42
OUTPUT_PATH = "E:/mars/Assets/MarsPrototype/Textures/mars_heightmap_placeholder.png"

random.seed(SEED)


def clamp(v, lo=0.0, hi=1.0):
    return max(lo, min(hi, v))


def smoothstep(edge0, edge1, x):
    t = clamp((x - edge0) / (edge1 - edge0))
    return t * t * (3.0 - 2.0 * t)


def generate_noise_field(size, octaves=6, persistence=0.5, base_freq=4.0):
    waves_per_octave = 4
    wave_params = []
    for octave in range(octaves):
        freq = base_freq * (2.0 ** octave)
        amp = persistence ** octave
        for _ in range(waves_per_octave):
            angle = random.uniform(0, 2 * math.pi)
            phase = random.uniform(0, 2 * math.pi)
            dx = math.cos(angle)
            dy = math.sin(angle)
            wave_params.append((freq, amp, dx, dy, phase))

    total_amp = sum(amp for _, amp, _, _, _ in wave_params)
    field = [[0.0] * size for _ in range(size)]
    inv_size = 1.0 / size

    for y in range(size):
        ny = y * inv_size
        for x in range(size):
            nx = x * inv_size
            val = 0.0
            for freq, amp, dx, dy, phase in wave_params:
                val += amp * math.sin(2 * math.pi * freq * (dx * nx + dy * ny) + phase)
            field[y][x] = val / total_amp
    return field


def add_crater(field, cx, cy, radius, depth, rim_height, size):
    x0 = max(0, int(cx - radius * 1.6))
    x1 = min(size, int(cx + radius * 1.6) + 1)
    y0 = max(0, int(cy - radius * 1.6))
    y1 = min(size, int(cy + radius * 1.6) + 1)

    for y in range(y0, y1):
        for x in range(x0, x1):
            ddx = x - cx
            ddy = y - cy
            dist = math.sqrt(ddx * ddx + ddy * ddy)
            t = dist / radius
            if t < 0.8:
                bowl = smoothstep(0.0, 0.8, t)
                field[y][x] -= depth * (1.0 - bowl)
            elif t < 1.0:
                wall = smoothstep(0.8, 1.0, t)
                field[y][x] += rim_height * math.sin(wall * math.pi)
            elif t < 1.5:
                falloff = smoothstep(1.0, 1.5, t)
                field[y][x] += rim_height * (1.0 - falloff) * 0.4


def populate_craters(field, size, count_large=5, count_medium=15, count_small=40):
    crater_specs = []
    for _ in range(count_large):
        r = random.randint(60, 120)
        cx = random.randint(r, size - r)
        cy = random.randint(r, size - r)
        depth = random.uniform(0.12, 0.22)
        rim = random.uniform(0.04, 0.08)
        crater_specs.append((cx, cy, r, depth, rim))
    for _ in range(count_medium):
        r = random.randint(25, 55)
        cx = random.randint(r, size - r)
        cy = random.randint(r, size - r)
        depth = random.uniform(0.08, 0.15)
        rim = random.uniform(0.03, 0.06)
        crater_specs.append((cx, cy, r, depth, rim))
    for _ in range(count_small):
        r = random.randint(8, 22)
        cx = random.randint(r, size - r)
        cy = random.randint(r, size - r)
        depth = random.uniform(0.05, 0.10)
        rim = random.uniform(0.02, 0.04)
        crater_specs.append((cx, cy, r, depth, rim))
    for cx, cy, r, depth, rim in crater_specs:
        add_crater(field, cx, cy, r, depth, rim, size)


def add_ridge(field, size, x0, y0, x1, y1, width, height, segments=60):
    perp_x = -(y1 - y0)
    perp_y = (x1 - x0)
    length = math.sqrt(perp_x ** 2 + perp_y ** 2) + 1e-9
    perp_x /= length
    perp_y /= length
    points = []
    for i in range(segments + 1):
        t = i / segments
        bx = x0 + (x1 - x0) * t
        by = y0 + (y1 - y0) * t
        wobble = random.gauss(0, width * 0.3)
        points.append((bx + perp_x * wobble, by + perp_y * wobble))
    sigma2 = 2.0 * (width * 0.5) ** 2
    for px, py in points:
        ix0 = max(0, int(px - width * 2))
        ix1 = min(size, int(px + width * 2) + 1)
        iy0 = max(0, int(py - width * 2))
        iy1 = min(size, int(py + width * 2) + 1)
        for y in range(iy0, iy1):
            for x in range(ix0, ix1):
                ddx = x - px
                ddy = y - py
                d2 = ddx * ddx + ddy * ddy
                contrib = height * math.exp(-d2 / sigma2)
                field[y][x] += contrib


def populate_ridges(field, size, count=4):
    for _ in range(count):
        x0 = random.randint(0, size)
        y0 = random.randint(0, size)
        angle = random.uniform(0, 2 * math.pi)
        rlength = random.randint(size // 4, size // 2)
        x1 = int(x0 + math.cos(angle) * rlength)
        y1 = int(y0 + math.sin(angle) * rlength)
        width = random.uniform(15, 40)
        height = random.uniform(0.06, 0.14)
        add_ridge(field, size, x0, y0, x1, y1, width, height)


def generate():
    print(f"Generating {SIZE}x{SIZE} Mars heightmap ...")

    print("  [1/4] Generating base noise field (rolling hills) ...")
    field = generate_noise_field(SIZE, octaves=6, persistence=0.5, base_freq=3.0)

    print("  [2/4] Adding secondary noise layer ...")
    field2 = generate_noise_field(SIZE, octaves=4, persistence=0.45, base_freq=7.0)
    for y in range(SIZE):
        for x in range(SIZE):
            field[y][x] = field[y][x] * 0.7 + field2[y][x] * 0.3

    print("  [3/4] Carving ridges and mountains ...")
    populate_ridges(field, SIZE, count=5)

    print("  [4/4] Stamping craters ...")
    populate_craters(field, SIZE, count_large=6, count_medium=18, count_small=50)

    print("  Normalizing to 0-255 ...")
    lo = float("inf")
    hi = float("-inf")
    for y in range(SIZE):
        for x in range(SIZE):
            v = field[y][x]
            if v < lo:
                lo = v
            if v > hi:
                hi = v
    rng = hi - lo if hi != lo else 1.0

    pixels = bytearray(SIZE * SIZE)
    idx = 0
    for y in range(SIZE):
        for x in range(SIZE):
            v = (field[y][x] - lo) / rng
            v = v ** 0.9
            pixels[idx] = int(clamp(v) * 255)
            idx += 1

    img = Image.frombytes("L", (SIZE, SIZE), bytes(pixels))
    img.save(OUTPUT_PATH)
    print(f"  Saved heightmap to: {OUTPUT_PATH}")
    print("Done.")


if __name__ == "__main__":
    generate()
