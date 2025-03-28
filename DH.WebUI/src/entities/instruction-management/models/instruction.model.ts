export interface Instruction {
  header: string;
  description: string;
  imagePath: string;
  links: Link[];
}

export interface Link {
  name: string;
  path: string;
  image: string;
  description: string;
}
