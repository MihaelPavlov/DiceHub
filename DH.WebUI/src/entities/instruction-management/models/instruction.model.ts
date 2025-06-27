export interface InstructionSection {
  title: string;
  summary: string;
  imageUrl: string;
  topics: InstructionTopic[];
}

export interface InstructionTopic {
  title: string;
  route: string;
  thumbnailUrl: string;
  description: string;
  steps: InstructionStep[];
}

export interface InstructionStep {
  header: string;
  description: string;
  mediaUrl: string | null;
  action?: StepActionLink | null;
}

export interface StepActionLink {
  label: string;
  url: string;
}

export enum LinkInfoType {
  Empty,
  Header,
  Text,
  Link,
  Image,
  Video,
}
