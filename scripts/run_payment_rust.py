#!/usr/bin/env python3
"""Run the single-source Rust sample with stable Cargo and a temporary manifest."""
from pathlib import Path
import json
import subprocess
import tempfile
import sys

source = (Path(sys.argv[1]).resolve() if len(sys.argv) > 1 else
          Path(__file__).resolve().parents[1] / 'samples/file-based/Payment.rs')
with tempfile.TemporaryDirectory(prefix='cupj-payment-rust-') as directory:
    manifest = Path(directory) / 'Cargo.toml'
    manifest.write_text('''[package]
name = "playground-payment-sample"
version = "0.0.0"
edition = "2024"
publish = false

[dependencies]
serde = { version = "1", features = ["derive"] }
serde_json = "1"

[[bin]]
name = "payment"
path = ''' + json.dumps(str(source)) + '\n')
    subprocess.run(['cargo', 'run', '--quiet', '--manifest-path', str(manifest)], check=True)
