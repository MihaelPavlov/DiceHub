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
  linkInfo: LinkInfo[];
}

export interface LinkInfo {
  header:string,
  description:string,
  link: string | null,
  imagePath: string,
}

export enum LinkInfoType {
  Empty,
  Header,
  Text,
  Link,
  Image,
  Video,
}
