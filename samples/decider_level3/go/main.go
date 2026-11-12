package main

import (
	"encoding/json"
	"fmt"
	"io"
	"os"
	"strconv"
)

func main() {
	if len(os.Args) == 3 && os.Args[1] == "--projection-scale" {
		n, err := strconv.Atoi(os.Args[2])
		if err != nil {
			panic(err)
		}
		projectionTests(n)
		return
	}
	if err := run(); err != nil {
		fmt.Fprintln(os.Stderr, err)
		os.Exit(1)
	}
}
func run() error {
	data, err := io.ReadAll(os.Stdin)
	if err != nil {
		return err
	}
	input, err := decodeInput(data)
	if err != nil {
		return err
	}
	output, err := runSchool(input.History, input.Operations)
	if err != nil {
		return err
	}
	return json.NewEncoder(os.Stdout).Encode(output)
}
