import {
  Box,
  ChakraProvider,
  createSystem,
  defineConfig,
  Text,
} from "@chakra-ui/react";

const config = defineConfig({
  theme: {
    tokens: {
      colors: {},
    },
  },
});

const system = createSystem(config);

export default function MobileMainPage() {


  return (
    <ChakraProvider value={system}>
      <Box textAlign="center" p={10}>
        <Text fontSize="xl">Telegram Web App</Text>
      </Box>
    </ChakraProvider>
  );
}
